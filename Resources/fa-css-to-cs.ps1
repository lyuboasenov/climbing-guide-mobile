# Css taken from fontawsesome.css / web package

# expected format "arrows-alt-h = "\uF337";"
# output          "public const string ArrowsAltH = "\uF337";"


$lines = @()
$textInfo = (New-Object System.Globalization.CultureInfo @('en-Us', $false) ).TextInfo

foreach($line in Get-Content .\fa.cs) {
    $nameAndValue = $line.Split('=')
    $name = $nameAndValue[0].Trim()
    $value = $nameAndValue[1].Trim()

    $name = $name.Replace('-', ' ')
    $name = $textInfo.ToTitleCase($name)
    $name = $name.Replace(' ', '')

    $lines += "public const string $name = $value"
}

$lines | Set-Content ./fa-fixed.cs