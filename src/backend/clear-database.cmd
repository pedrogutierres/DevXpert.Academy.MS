@echo off
set "projetoCaminho=.\"

echo Procurando arquivos .db em %projetoCaminho% ...

for /r %projetoCaminho% %%f in (*.db) do (
    if exist "%%f" (
        echo Excluindo arquivo: %%f
        del /q "%%f"
    )
)

for /r %projetoCaminho% %%f in (*.db-shm) do (
    if exist "%%f" (
        echo Excluindo arquivo: %%f
        del /q "%%f"
    )
)

for /r %projetoCaminho% %%f in (*.db-wal) do (
    if exist "%%f" (
        echo Excluindo arquivo: %%f
        del /q "%%f"
    )
)

echo.
echo ✅ Concluído.
pause
exit
