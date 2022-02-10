<#
.SYNOPSIS
    Re-writes every file in a directory so that:
      - All of lines end in `\n` (aka [https://en.wikipedia.org/wiki/Newline#Representation](LF))
      - There is no trailing newline character at the end of the file
      - The encoding is [https://en.wikipedia.org/wiki/Byte_order_mark](UTF8NoBOM)
#>
function Fix-LineEndings(
    # The path or paths to the files or directories that will have their line endings fixed.
    #   - Passed through as the `Get-ChildItem -Path` parameter.
    [Parameter(ValueFromPipeline)]
    [string[]]$Path = ".",

    # Allows you to _opt-out_ of the `Get-ChildItem`'s `-Recurse` option.
    [switch]$NoRecurse
){
    $shouldRecur = !$NoRecurse
    $files = Get-ChildItem -Path $Path -Recurse:$shouldRecur -File
    foreach($f in $files){
        $lines = $f | Get-Content
        $joined = $lines -join "`n"
        Set-Content -Path $f -Value $joined -NoNewLine -Encoding 'UTF8NoBOM'
    }
}