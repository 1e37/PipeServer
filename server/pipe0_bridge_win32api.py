import win32file
import pywintypes

# Define the name of the existing pipe
pipe_name = r'\\.\pipe\pipe0'

# Connect to the pipe
handle = win32file.CreateFile(pipe_name,
                              win32file.GENERIC_READ | win32file.GENERIC_WRITE,
                              0, None, win32file.OPEN_EXISTING, 0, None)

if handle == win32file.INVALID_HANDLE_VALUE:
    print("Failed to connect to pipe!")
else:
    print("Connected to pipe!")
    win32file.WriteFile(handle, b"Hello from Frontend!")

    while True:
        try:
            # Read from the pipe
            line = win32file.ReadFile(handle, 1024)
            print(f"Received from Server: {line}")

            # Send a response back to the server
            
        except pywintypes.error as e:
            print(f"Error: {e}")
            break

    # Close the pipe
    win32file.CloseHandle(handle)