$loop = 1;
while ($loop)
{
    $process = Start-Process "\Booper.exe" -Wait -NoNewWindow -PassThru
    switch ($process.ExitCode)
    {
        0 {"Restarting..."; Start-Sleep -s 3}
        1 {"Exiting."; $loop = 0;}
        default {"Unhandled Exit Code, Exiting."; $loop = 0}
    }
}
# In your bot, you can use Environment.Exit(0) to restart, or Environment.Exit(1) to kill.
# Place this file in with the exe or amend the process name to include the path.