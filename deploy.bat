@echo off
cd "%1"
if exist deploy\deploy.bat (
	echo Calling extra deploy.bat
	cd deploy
	call deploy.bat "%1" "%2"
) else (
	echo Done
)
