## SERVER.PY

import socket
import time

class Server:
    def __init__(self, host, port):
        self.host = host
        self.port = port
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.success = self.connect_to_server()
        
    def connect_to_server(self):
        no_of_retries = 5
        interval = 5
        
        attempts = 0
        success = False
        while attempts < no_of_retries:
            try:
                self.server_socket.connect((self.host, self.port))
                print('Connection Established:', self.host, ':', self.port)
                success = True
                return success 
            except ConnectionRefusedError:
                print(f'Connection Refused on Attempt ({attempts + 1}/{no_of_retries}) Retrying...')
                time.sleep(interval)
                attempts += 1
        
        return success
    
    def send_to_unity(self, message):
        try:
            self.server_socket.sendall(message.encode("utf-8"))
        except Exception as e:
            print(f'Error sending data to Unity: {e}')
