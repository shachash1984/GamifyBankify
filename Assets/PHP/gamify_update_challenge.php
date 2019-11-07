<?php
include 'dbconnect.php';


$pin = mysqli_real_escape_string($dblink,$_POST['PIN']);
$userContainer = mysqli_real_escape_string($dblink,$_POST['userContainerString']);

$query = "UPDATE gamify_players SET userContainer='$userContainer' WHERE PIN='$pin'";

$result = mysqli_query($dblink, $query);

if($result){

	$dataArray = array('success' => true, 'error' => '');	
	
}
else{
	$dataArray = array('success' => false, 'error' => 'could not connect to server');
}

header('Content-Type: application/json');
echo json_encode($dataArray);