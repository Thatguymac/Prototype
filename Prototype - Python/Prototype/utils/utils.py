# UTILS

import torch

class Utils:
    def get_device():
        device = torch.device("cuda" if torch.cuda.is_available() else "cpu")      
        print(f'Using: {device}')        

        return device
    