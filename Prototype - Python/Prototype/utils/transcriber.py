# TRANSCRIBER

import threading
import whisper
import time
import queue

class Transcriber:
    def __init__(self, model, options, server, processor, recorder):
        self.server = server
        self.processor = processor
        self.recorder = recorder
        
        self.model = model
        self.options = options
        
        self.thread_running = False
        self.thread = None
        self.lock = threading.Lock()
    
    def start_transcribing(self):
        if not self.thread_running:
            self.thread_running = True
            self.thread = threading.Thread(target=self.transcribing_thread, args=(self.recorder.recordings,), daemon=True)
            self.thread.start()
    
    def stop_transcribing(self):
        if self.thread_running:
            self.thread_running = False
            if self.thread is not None:
                self.thread.join()
                self.thread = None 
    
    def transcribing_thread(self, recordings):
        while self.thread_running:
            try:
                if not recordings.empty():
                    path = recordings.get()
                    audio = whisper.load_audio(path)
                    audio = whisper.pad_or_trim(audio)
                    mel = whisper.log_mel_spectrogram(audio).to(self.model.device)
                    result = whisper.decode(self.model, mel, self.options)
                    
                    processed_result = self.processor.preprocess_text(result.text)

                    for r in processed_result:
                        self.server.send_to_unity(r)
                        time.sleep(0.75)
                
                    recordings.task_done()
            except Exception as e:
                print(e)
                
            time.sleep(1)
            
        self.clear_queue(recordings)
        
    def clear_queue(self, recordings):
        try:
            while True:
                recordings.get_nowait()
                recordings.task_done()
        except queue.Empty:
            pass