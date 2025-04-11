$rootCertName = "LocalhostDevRootCA"
$localhostCertName = "localhost"
$pfxPassword = ConvertTo-SecureString -String "WslLocalhost" -Force -AsPlainText
$pfxPath = "$env:USERPROFILE\.aspnet\https\WslLocalhost.pfx"

# 古い証明書を削除
Get-ChildItem Cert:\CurrentUser\My | Where-Object { $_.Subject -like "*$rootCertName*" -or $_.Subject -like "*$localhostCertName*" } | Remove-Item -Force
Get-ChildItem Cert:\LocalMachine\Root | Where-Object { $_.Subject -like "*$rootCertName*" } | Remove-Item -Force

# 自己署名ルートCA作成（CA用に Type: Custom + KeyUsage: CertSign）
$rootCert = New-SelfSignedCertificate `
  -Subject "CN=$rootCertName" `
  -CertStoreLocation "Cert:\CurrentUser\My" `
  -KeyExportPolicy Exportable `
  -KeyUsage CertSign, CRLSign, DigitalSignature `
  -KeyAlgorithm RSA `
  -KeyLength 2048 `
  -HashAlgorithm sha256 `
  -NotAfter (Get-Date).AddYears(10) `
  -Type Custom

# localhost用の証明書（上記ルートCAで署名）
$serverCert = New-SelfSignedCertificate `
  -Subject "CN=localhost" `
  -DnsName "localhost" `
  -CertStoreLocation "Cert:\CurrentUser\My" `
  -KeyExportPolicy Exportable `
  -KeyUsage DigitalSignature, KeyEncipherment `
  -KeyAlgorithm RSA `
  -KeyLength 2048 `
  -HashAlgorithm sha256 `
  -NotAfter (Get-Date).AddYears(5) `
  -Signer $rootCert `
  -Type SSLServerAuthentication

# PFXファイルにエクスポート
Export-PfxCertificate -Cert $serverCert -FilePath $pfxPath -Password $pfxPassword

# ルートCAを信頼されたルート証明機関に追加
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store("Root", "LocalMachine")
$store.Open("ReadWrite")
$store.Add($rootCert)
$store.Close()

# WSL用の環境変数も設定
[System.Environment]::SetEnvironmentVariable("WSLENV", "USERPROFILE/up", [System.EnvironmentVariableTarget]::User)
