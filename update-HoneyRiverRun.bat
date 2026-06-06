@echo off
echo ========================================
echo Updating Honey River Run on itch.io
echo ========================================

cd /d "C:\Users\danie\Documents\Unity Projects\HoneyRiverRun"

echo.
echo Pushing WebGL build to itch.io...
butler push "Build\WebGL" mrcanela/honey-river-run:webgl --userversion %1

echo.
echo Done! Game updated on itch.io.
echo.
pause