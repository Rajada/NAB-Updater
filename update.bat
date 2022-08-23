@echo off
del "Nerf ArenaBlast Updater.exe"
ren "Nerf ArenaBlast Updater.exe.new" "Nerf ArenaBlast Updater.exe"
start "" "Nerf ArenaBlast Updater.exe"
start /b "" cmd /c del "%~f0"&exit /b
