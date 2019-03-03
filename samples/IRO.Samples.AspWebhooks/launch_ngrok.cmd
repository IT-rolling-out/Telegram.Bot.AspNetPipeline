echo "Install ngrok.exe and add to system PATH."
call ngrok http -host-header="localhost:52860" 52860
pause