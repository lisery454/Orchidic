# Scripts/fetch-fonts.ps1

# -----------------------------
# 配置
# -----------------------------
$zipUrl = "https://hyperos.mi.com/font-download/MiSans.zip"
$zipFile = "MiSans.zip"
$fontsDir = "Orchidic\Assets\Fonts"

# -----------------------------
# 创建目录
# -----------------------------
if (-Not (Test-Path $fontsDir)) {
    Write-Host "Creating Fonts directory at $fontsDir"
    New-Item -ItemType Directory -Path $fontsDir | Out-Null
}

# -----------------------------
# 下载 ZIP 文件
# -----------------------------
Write-Host "Downloading $zipUrl ..."
Invoke-WebRequest -Uri $zipUrl -OutFile $zipFile
Write-Host "Download completed."

# -----------------------------
# 解压 ZIP 文件到临时目录
# -----------------------------
$tempDir = "$fontsDir\temp_extract"
if (Test-Path $tempDir) { Remove-Item $tempDir -Recurse -Force }

Write-Host "Extracting $zipFile to temporary directory..."
Expand-Archive -Path $zipFile -DestinationPath $tempDir -Force

# -----------------------------
# 移动字体文件到目标 Fonts 目录
# -----------------------------
Write-Host "Moving TTF font files to $fontsDir ..."
Get-ChildItem -Path $tempDir -Recurse -File -Filter *.ttf | ForEach-Object {
    Move-Item -Path $_.FullName -Destination $fontsDir -Force
}

# 删除临时目录
Remove-Item $tempDir -Recurse -Force

# -----------------------------
# 删除 ZIP 文件
# -----------------------------
Remove-Item $zipFile
Write-Host "$zipFile removed."

Write-Host "Fonts setup completed successfully!"
