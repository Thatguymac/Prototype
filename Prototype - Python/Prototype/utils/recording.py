import queue
import threading
import sounddevice as sd
import numpy as np
import wavio as wv
import os
import datetime

class Recording():
    def __init__(self, sample_rate=16000, channels=1, path=r"E:\UnityRecordings", silence_threshold=-40, silence_duration=1):
        self.sample_rate = sample_rate
        self.channels = channels
        self.path = path
        self.silence_threshold = silence_threshold
        self.silence_duration = silence_duration
        
        # Use lists for initial data collection for efficiency
        self.recording_buffer = []
        self.silence_buffer = []
        
        self.last_sound_time = datetime.datetime.now()
        self.recordings = queue.Queue()

        self.thread_running = False
        self.thread = None
        self.lock = threading.Lock()
        
        os.makedirs(path, exist_ok=True)

    def start_recording(self):
        if not self.thread_running:
            self.thread_running = True
            self.recording_buffer = []
            self.silence_buffer = []
            self.thread = threading.Thread(target=self.recording_thread, daemon=True)
            self.thread.start()

    def stop_recording(self):
        if self.thread_running:
            self.thread_running = False
            if self.thread is not None:
                self.thread.join()  # Wait for the recording thread to finish
                self.thread = None
        
    def recording_thread(self):
        with sd.InputStream(callback=self.audio_callback, 
                            channels=self.channels, 
                            samplerate=self.sample_rate,
                            blocksize=2048,  # Adjusted blocksize
                            latency='low'):  # Low latency setting
            while self.thread_running:
                sd.sleep(100)  # Small sleep to prevent this loop from hogging CPU

    def audio_callback(self, indata, frames, time, status):
        if status:
            print("Status:", status)
        with self.lock:
            if self.is_silence(indata):
                if (datetime.datetime.now() - self.last_sound_time).total_seconds() > self.silence_duration:
                    self.save_recording()  # Save the current sentence if silence persists
                    self.recording_buffer = []
                self.silence_buffer.append(indata.copy())
            else:
                self.recording_buffer.append(indata.copy())
                self.last_sound_time = datetime.datetime.now()

    def is_silence(self, data):
        rms = np.sqrt(np.mean(np.square(data)))
        rms_dB = 20 * np.log10(rms) if rms > 0 else -np.inf
        return rms_dB < self.silence_threshold

    def save_recording(self):
        if self.recording_buffer:
            recording_array = np.vstack(self.recording_buffer)  # Convert list to numpy array here
            filename = datetime.datetime.now().strftime("%Y-%m-%d_%H-%M-%S.wav")
            file_path = os.path.join(self.path, filename)
            wv.write(file_path, recording_array, self.sample_rate, sampwidth=2)
            self.recordings.put(file_path)
            print(f"Recording saved to {file_path}")
