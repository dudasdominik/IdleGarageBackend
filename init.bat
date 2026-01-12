@echo off
setlocal enabledelayedexpansion

:: Always run from this .bat's directory (fix relative paths)
cd /d "%~dp0"

:: Config - container name and volume
set CONTAINER_NAME=postgres_container
set VOLUME_NAME=garageclickerbackend_postgres_data

echo ----------------------------------------
echo [STEP 0] Cleaning up existing container and volume...

docker rm -f %CONTAINER_NAME% >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Removed existing container: %CONTAINER_NAME%
) else (
    echo No existing container to remove.
)

docker volume rm %VOLUME_NAME% >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Removed volume: %VOLUME_NAME%
) else (
    echo No volume to remove or already gone.
)

echo.
echo ----------------------------------------
echo [STEP 1] Loading environment variables from .env...

if not exist ".env" (
    echo ❌ .env file not found in: %cd%
    pause
    exit /b 1
)

for /f "usebackq tokens=1,* delims==" %%A in (".env") do (
    echo %%A| findstr /b "#" >nul
    if errorlevel 1 (
        set "%%A=%%B"
    )
)

echo PG_USER = %PG_USER%
echo PG_PASSWORD = %PG_PASSWORD%
echo PG_DATABASE = %PG_DATABASE%
echo PG_PORT = %PG_PORT%
echo.

echo ----------------------------------------
echo [STEP 2] Starting Docker Compose...

docker-compose --env-file .env up -d
IF %ERRORLEVEL% NEQ 0 (
    echo ❌ Failed to start Docker Compose.
    pause
    exit /b 1
)

echo Docker Compose started.

echo ----------------------------------------
echo [STEP 2.5] Waiting for PostgreSQL to accept connections...

:: Wait loop (max ~30s)
set /a TRY=0
:WAIT_DB
set /a TRY+=1

docker exec -i %CONTAINER_NAME% pg_isready -U %PG_USER% -d %PG_DATABASE% >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo ✅ PostgreSQL is ready.
    goto DB_READY
)

if %TRY% GEQ 15 (
    echo ❌ PostgreSQL did not become ready in time.
    echo Showing last logs:
    docker logs %CONTAINER_NAME% --tail 80
    pause
    exit /b 1
)

ping -n 3 127.0.0.1 >nul
goto WAIT_DB

:DB_READY
echo.

echo ----------------------------------------
echo [STEP 3] Running EF Core migration...

dotnet ef database update
IF %ERRORLEVEL% NEQ 0 (
    echo ❌ EF Core migration failed.
    pause
    exit /b 1
)

echo ✅ EF Core migration completed.
echo.


echo.
echo ✅ All done. PostgreSQL is running and DB is migrated + (init)seeded!
pause
endlocal
