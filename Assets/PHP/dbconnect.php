<?php
$dbuser = '*';
$dbpassword = '*';
$db = '*';
$dbhost = 'localhost';

$dbport = *;

$dblink = mysqli_init();
$dbconnection = mysqli_real_connect($dblink, $dbhost, $dbuser, $dbpassword, $db, $dbport);


/*if($dbconnection){
	print("success");
}
else{
	die("Connection failed " . mysqli_error());
}*/

?>
