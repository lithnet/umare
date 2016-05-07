function Transform-Values
{
	param ($items)

	foreach ($item in $items)
	{
		[Array]::Reverse($item)
		write-output $item -NoEnumerate
	}
}