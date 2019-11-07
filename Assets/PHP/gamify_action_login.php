<?php

include_once 'dbconnect.php';


$pin = mysqli_real_escape_string($dblink,$_POST['PIN']);


$query = "SELECT gameIndex, PIN, userContainer  FROM gamify_players WHERE PIN='$pin'";

$result = mysqli_query($dblink, $query);


if($result){
$row = mysqli_fetch_row($result);
	if($row){
		
		$dataArray = array('success' => true, 'error' => '');
	} 
	
	else{
		$dataArray = array('success' => false, 'error' => 'PIN code is not correct');
	}
}
else{
	$dataArray = array('success' => false, 'error' => 'Could not connect to server');
}
header('Content-Type: application/json');
echo json_encode($dataArray);
?>