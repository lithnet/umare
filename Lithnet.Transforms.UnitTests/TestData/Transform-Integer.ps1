param (
    $items
    )

foreach ($item in $items)
{
    write-output ($item + 1)
}