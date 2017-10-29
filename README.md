
#Network client using C# and XAML. Send request to the server and receive response from server


Request Structure

Name	Length	

Header	2 bytes	
Version	1 byte	
Length	4 bytes	
Operation	1 byte	
Data	Variable, 0 to 1,048,576 bytes
Checksum	1 byte	


Response Structure

Name	Length	

Header	2 bytes	
Status	1 byte	
Length	4 bytes	
Data	Variable, 0 to 1,048,576 bytes	
Checksum	1 byte	
