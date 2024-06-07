# PROTOTYPE

import whisper

from utils.server import Server
from utils.utils import Utils
from utils.recording import Recording
from utils.transcriber import Transcriber
from utils.processor import Processor

def start_server():
    host = '127.0.0.1'
    port = 25001
    
    return Server(host, port)

def main_thread():
    model = whisper.load_model('medium', Utils.get_device())
    options = whisper.DecodingOptions(language='en', without_timestamps=True, fp16=False)
    
    server = start_server()
    processor = Processor()
    
    recording = Recording()
    transcribing = Transcriber(model, options, server, processor, recording)
    
    data = ''

    while True:
        if server.success:
            try:
                data = server.server_socket.recv(1024).decode("utf-8")
            except Exception as e:
                print(f'There was an error when retrieving data from Unity: {str(e)}')

            if data == 'Start_Recording':
                recording.start_recording()
                transcribing.start_transcribing()

            elif data == 'Stop_Recording':
                recording.stop_recording()
                transcribing.stop_transcribing()

            elif data == 'Quit_From_Unity':
                recording.stop_recording()
                transcribing.stop_transcribing()

                print('Waiting for threads to stop...')
                print('Closing python server...')
                break
        else:
            print('Connection Unsuccessful')
            break

if __name__ == "__main__":
    main_thread()