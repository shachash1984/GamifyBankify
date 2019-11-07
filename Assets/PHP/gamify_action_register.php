<?php
include 'dbconnect.php';



$game = mysqli_real_escape_string($dblink,$_POST['gameIndex']);
$pin = mysqli_real_escape_string($dblink,$_POST['PIN']);
$userContainer = mysqli_real_escape_string($dblink,$_POST['userContainer']);


$query = "SELECT userContainer FROM gamify_players WHERE PIN='$pin'";

$result = mysqli_query($dblink, $query);

$row = mysqli_fetch_row($result);
if($row[0] == $userContainer){
	$dataArray = array('success' => false, 'error' => 'challenge already exists.', 'gameIndex' => "$game");
} else {
	$query2 = "INSERT INTO gamify_players(gameIndex, PIN, userContainer) VALUES ('$game', '$pin', '$userContainer')";
	if($result2 = mysqli_query($dblink, $query2)){
		$id = mysqli_insert_id($dblink);
		$dataArray = array('success' => true, 'error' => '' , 'gameIndex' => "$game", 'PIN' => "$pin", 'userContainerJSON' => "$userContainer");
	}
	else{
		$dataArray = array('success' => false, 'error' => 'Could not create user, try again later.', 'game' => "$gameIndex", 'PIN' => "$pin");
	}
}

header('Content-Type: application/json');
echo json_encode($dataArray);



?>