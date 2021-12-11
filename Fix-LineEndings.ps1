function Fix-LineEndings(
    $Path = "."
){
    $files = Get-ChildItem . -Recurse -File
    foreach($f in $files){
        $lines = $f | Get-Content
        $joined = $lines -join "`n"
        Set-Content -Path $f -Value $joined -NoNewLine -Encoding 'UTF8NoBOM'
    }
}