function Transform-Values
{
	param ($items)

	foreach ($item in $items)
	{
		write-output (!$item);
	}
}