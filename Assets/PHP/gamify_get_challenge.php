<?php
include 'dbconnect.php';


$pin = mysqli_real_escape_string($dblink,$_POST['PIN']);

$query = "SELECT gameIndex, PIN, userContainer FROM  gamify_players WHERE PIN='$pin'";


$result = mysqli_query($dblink, $query);

if($result){

	$row = mysqli_fetch_row($result);

	if($row){
		$gameIndex = $row[0];
		$pin = $row[1];
		$userContainer = $row[2];
		$dataArray = array('success' => true, 'error' => '', 'gameIndex' => "$gameIndex", 'PIN' => "$pin", 'userContainerJSON' => "$userContainer");	
	}

	else{
	$dataArray = array('success' => false, 'error' => 'Challenge not found');
	
	}
}
else{
	$dataArray = array('success' => false, 'error' => 'could not connect to server');
}

header('Content-Type: application/json');
echo json_encode($dataArray);