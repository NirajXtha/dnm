<?php

use PHPMailer\PHPMailer\PHPMailer;
use PHPMailer\PHPMailer\Exception;

$handle = fopen('AttdNotification.txt', 'w');
require '/apache24\htdocs\neo-hris\public\AttendanceScheduler/PHPMailer.php';
require '/apache24\htdocs\neo-hris\public\AttendanceScheduler/SMTP.php';
$mail = new PHPMailer(true);
$mailApp = new PHPMailer(true);
$mailRec = new PHPMailer(true);
$mailSelf = new PHPMailer(true);


$mail->isSMTP();
$mail->Host = 'smtp.gmail.com';
$mail->Port = 587;
$mail->SMTPSecure = 'TLS';
$mail->SMTPAuth = true;
$mail->Username = 'neosoftwaredrive2@gmail.com';
$mail->Password = 'zuqpcelhydjpivjh';

$mailApp->isSMTP();
$mailApp->Host = 'smtp.gmail.com';
$mailApp->Port = 587;
$mailApp->SMTPSecure = 'TLS';
$mailApp->SMTPAuth = true;
$mailApp->Username = 'neosoftwaredrive2@gmail.com';
$mailApp->Password = 'zuqpcelhydjpivjh';

$mailRec->isSMTP();
$mailRec->Host = 'smtp.gmail.com';
$mailRec->Port = 587;
$mailRec->SMTPSecure = 'TLS';
$mailRec->SMTPAuth = true;
$mailRec->Username = 'neosoftwaredrive2@gmail.com';
$mailRec->Password = 'zuqpcelhydjpivjh';

$mailSelf->isSMTP();
$mailSelf->Host = 'smtp.gmail.com';
$mailSelf->Port = 587;
$mailSelf->SMTPSecure = 'TLS';
$mailSelf->SMTPAuth = true;
$mailSelf->Username = 'neosoftwaredrive2@gmail.com';
$mailSelf->Password = 'zuqpcelhydjpivjh';



// Establish Connection
$dbhost = 'localhost';
$dbport = '1521';
$dbname = 'HRIS';
$dbuser = 'NEO_HRIS';
$dbpass = 'NEO_HRIS';
$connection = oci_connect($dbuser, $dbpass, "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=$dbhost)(PORT=$dbport))(CONNECT_DATA=(SERVICE_NAME=$dbname)))");

$mail->isHTML(true);
$mailApp->isHTML(true);
$mailRec->isHTML(true);
$mailSelf->isHTML(true);


// Check if the connection was successful
if (!$connection) {
    $error = oci_error();
    echo "Failed to connect to Oracle database: " . $error['message'];
    exit;
}


function sendEmail($mail, $subject, $recipientEmail, $recipientName, $body)
{
    $mail->setFrom(isset($row['EMAIL_OFFICIAL']) ? $row['EMAIL_OFFICIAL'] : 'neosoftwaredrive2@gmail.com', 'Admin');
    $mail->addAddress($recipientEmail, $recipientName);
    $mail->Subject = $subject;
    $mail->Body = $body;
    $mail->send();
}

function mailLog($subject, $recipientEmail, $recipientName, $connection)
{
    if (!$connection) {
        echo "Database connection is not established.";
        return;
    }
    $sql = "INSERT INTO hris_email_notification 
    (id, title, TO_NAME, to_email, created_dt) 
    VALUES 
    (hris_email_notification_seq.NEXTVAL, :subject, :recipientName, :recipientEmail, SYSDATE)";


    $statement = oci_parse($connection, $sql);

    oci_bind_by_name($statement, ':subject', $subject);
    oci_bind_by_name($statement, ':recipientName', $recipientName);
    oci_bind_by_name($statement, ':recipientEmail', $recipientEmail);

    oci_execute($statement);

    $error = oci_error($statement);

    if ($error) {
        echo "OCI Error: " . $error['message'];
    } else {
        echo "Mail sent successfully!";
    }
}



// // Late in Notification
$query5 = "SELECT
c.company_name,
e.employee_id,
e.email_official,
e.full_name,
e.mobile_no,
p.position_name,
d.department_name,
(SELECT full_name FROM hris_employees WHERE position_id = 55) AS APPROVER_NAME,
(SELECT 'sabita.magar@neosoftware.com.np' FROM hris_employees WHERE position_id = 55) AS APPROVER_EMAIL,
'1000605' AS APPROVED_BY,
(SELECT full_name FROM hris_employees WHERE position_id = 56) AS RECOMMENDER_NAME,
(SELECT 'sabita.magar@neosoftware.com.np' FROM hris_employees WHERE position_id = 56) AS RECOMMENDER_EMAIL,
'1000606' AS RECOMMEND_BY,
'1000477' AS DEFAULT_BY,
(SELECT full_name FROM hris_employees WHERE employee_id = 1000477) AS DEFAULT_NAME,
(SELECT 'yagya.raj@neosoftware.com.np' FROM hris_employees WHERE employee_id = 1000477) AS DEFAULT_EMAIL,
'1000547' AS HR_BY,
(SELECT full_name FROM hris_employees WHERE employee_id = 1000547) AS HR_NAME,
(SELECT 'yagya.raj@neosoftware.com.np' FROM hris_employees WHERE employee_id = 1000547) AS HR_EMAIL,
a.attendance_dt AS current_date,
TO_CHAR(a.IN_TIME, 'HH12:MI AM') AS ARRIVED_TIME
FROM
hris_employees e
LEFT JOIN
hris_attendance_detail a ON (a.employee_id = e.employee_id)
LEFT JOIN
hris_shifts H ON (a.shift_id = H.shift_id)
LEFT JOIN
HRIS_POSITIONS p ON (e.position_id = p.position_id)
LEFT JOIN
hris_departments d ON (e.department_id = d.department_id)
LEFT JOIN
hris_company c ON (e.company_id = c.company_id)
WHERE
a.attendance_dt = TRUNC(SYSDATE) -- Filter for today's date
AND a.two_day_shift = 'D'
AND (
    a.dayoff_flag = 'N' 
    AND e.status = 'E' 
    AND e.resigned_flag = 'N' 
    AND e.retired_flag = 'N' 
   -- AND TO_CHAR(a.IN_TIME, 'HH24:MI') > '12:30' -- IN_TIME is after 12:30 PM
)
AND a.overall_status = 'AB' 
AND a.IN_TIME IS NULL order by c.company_id ";
$statement = oci_parse($connection, $query5);
$result = oci_execute($statement);  // Execute the statement
// Initialize arrays to store late employee names for both approver and recommender


$recAppArray = [];
while ($row = oci_fetch_assoc($statement)) {



    
    $subject = "Absent Notification";

if ($row['APPROVED_BY'] == $row['RECOMMEND_BY']) {
    if(isset($recAppArray[$row['APPROVED_BY']])){
        array_push($recAppArray[$row['APPROVED_BY']]['data'],$row);
        array_push($recAppArray[$row['APPROVED_BY']]['employeesCsv']['employeesMobile'],$row['FULL_NAME'],$row['MOBILE_NO']);
        // array_push($recAppArray[$row['APPROVED_BY']]['employeesLateTime'],$row['MIN_LATE']);
    }else{
        $recAppArray[$row['APPROVED_BY']]['data'] =[$row];
        $recAppArray[$row['APPROVED_BY']]['email'] =$row['RECOMMENDER_EMAIL'];
        $recAppArray[$row['APPROVED_BY']]['name'] =$row['APPROVER_NAME'];
        $recAppArray[$row['APPROVED_BY']]['employeesCsv']['employeesMobile'] =[$row['FULL_NAME'],$row['MOBILE_NO']];
        // $recAppArray[$row['APPROVED_BY']]['employeesLateTime']=$row['MIN_LATE'];
    }
    if(isset($recAppArray[$row['DEFAULT_BY']])){
        array_push($recAppArray[$row['DEFAULT_BY']]['data'],$row);
        array_push($recAppArray[$row['DEFAULT_BY']]['employeesCsv']['employeesMobile'],$row['FULL_NAME'],$row['MOBILE_NO']);
        // array_push($recAppArray[$row['APPROVED_BY']]['employeesLateTime'],$row['MIN_LATE']);
    }else{
        $recAppArray[$row['DEFAULT_BY']]['data'] =[$row];
        $recAppArray[$row['DEFAULT_BY']]['email'] =$row['DEFAULT_EMAIL'];
        $recAppArray[$row['DEFAULT_BY']]['name'] =$row['DEFAULT_NAME'];
        $recAppArray[$row['DEFAULT_BY']]['employeesCsv']['employeesMobile'] =[$row['FULL_NAME'],$row['MOBILE_NO']];
        // $recAppArray[$row['APPROVED_BY']]['employeesLateTime']=$row['MIN_LATE'];
    }
	
	
	 if(isset($recAppArray[$row['HR_BY']])){
        array_push($recAppArray[$row['HR_BY']]['data'],$row);
        array_push($recAppArray[$row['HR_BY']]['employeesCsv']['employeesMobile'],$row['FULL_NAME'],$row['MOBILE_NO']);
        // array_push($recAppArray[$row['APPROVED_BY']]['employeesLateTime'],$row['MIN_LATE']);
    }else{
        $recAppArray[$row['HR_BY']]['data'] =[$row];
        $recAppArray[$row['HR_BY']]['email'] =$row['HR_EMAIL'];
        $recAppArray[$row['HR_BY']]['name'] =$row['HR_NAME'];
        $recAppArray[$row['HR_BY']]['employeesCsv']['employeesMobile'] =[$row['FULL_NAME'],$row['MOBILE_NO']];
        // $recAppArray[$row['APPROVED_BY']]['employeesLateTime']=$row['MIN_LATE'];
    }

    } else {
        if(isset($recAppArray[$row['APPROVED_BY']])){
            array_push($recAppArray[$row['APPROVED_BY']]['data'],$row);
            array_push($recAppArray[$row['APPROVED_BY']]['employeesCsv']['employeesMobile'],$row['FULL_NAME'],$row['MOBILE_NO']);
            // array_push($recAppArray[$row['APPROVED_BY']]['employeesLateTime'],$row['MIN_LATE']);
        }else{
            $recAppArray[$row['APPROVED_BY']]['data'] =[$row];
            $recAppArray[$row['APPROVED_BY']]['email'] =$row['APPROVER_EMAIL'];
            $recAppArray[$row['APPROVED_BY']]['name'] =$row['APPROVER_NAME'];
            $recAppArray[$row['APPROVED_BY']]['employeesCsv']['employeesMobile'] =[$row['FULL_NAME'],$row['MOBILE_NO']];
            // $recAppArray[$row['APPROVED_BY']]['employeesLateTime']=$row['MIN_LATE'];
        }
        if(isset($recAppArray[$row['RECOMMEND_BY']])){
            array_push($recAppArray[$row['RECOMMEND_BY']]['data'],$row);
            array_push($recAppArray[$row['RECOMMEND_BY']]['employeesCsv']['employeesMobile'],$row['FULL_NAME'],$row['MOBILE_NO']);
            // array_push($recAppArray[$row['RECOMMEND_BY']]['employeesLateTime'],$row['MIN_LATE']);
        }else{
            $recAppArray[$row['RECOMMEND_BY']]['data'] =[$row];
            $recAppArray[$row['RECOMMEND_BY']]['email'] =$row['RECOMMENDER_EMAIL'];
            $recAppArray[$row['RECOMMEND_BY']]['name'] =$row['RECOMMENDER_NAME'];
            $recAppArray[$row['RECOMMEND_BY']]['employeesCsv']['employeesMobile'] =[$row['FULL_NAME'], $row['MOBILE_NO']];
            // $recAppArray[$row['RECOMMEND_BY']]['employeesLateTime']=$row['MIN_LATE'];

        }
        if(isset($recAppArray[$row['DEFAULT_BY']])){
            array_push($recAppArray[$row['DEFAULT_BY']]['data'],$row);
            array_push($recAppArray[$row['DEFAULT_BY']]['employeesCsv']['employeesMobile'],$row['FULL_NAME'],$row['MOBILE_NO']);
            // array_push($recAppArray[$row['APPROVED_BY']]['employeesLateTime'],$row['MIN_LATE']);
        }else{
            $recAppArray[$row['DEFAULT_BY']]['data'] =[$row];
            $recAppArray[$row['DEFAULT_BY']]['email'] =$row['DEFAULT_EMAIL'];
            $recAppArray[$row['DEFAULT_BY']]['name'] =$row['DEFAULT_NAME'];
            $recAppArray[$row['DEFAULT_BY']]['employeesCsv']['employeesMobile'] =[$row['FULL_NAME'],$row['MOBILE_NO']];
            // $recAppArray[$row['APPROVED_BY']]['employeesLateTime']=$row['MIN_LATE'];
        }
		
		
		
		 if(isset($recAppArray[$row['HR_BY']])){
            array_push($recAppArray[$row['HR_BY']]['data'],$row);
            array_push($recAppArray[$row['HR_BY']]['employeesCsv']['employeesMobile'],$row['FULL_NAME'],$row['MOBILE_NO']);
            // array_push($recAppArray[$row['APPROVED_BY']]['employeesLateTime'],$row['MIN_LATE']);
        }else{
            $recAppArray[$row['HR_BY']]['data'] =[$row];
            $recAppArray[$row['HR_BY']]['email'] =$row['HR_EMAIL'];
            $recAppArray[$row['HR_BY']]['name'] =$row['HR_NAME'];
            $recAppArray[$row['HR_BY']]['employeesCsv']['employeesMobile'] =[$row['FULL_NAME'],$row['MOBILE_NO']];
            // $recAppArray[$row['APPROVED_BY']]['employeesLateTime']=$row['MIN_LATE'];
        }
      
    }


}

foreach ($recAppArray as $k => $v) {
$employeesMobileString = "";
foreach ($v['employeesCsv']['employeesMobile'] as $index => $employeesMobile) {
    // Check if index is odd (MIN_LATE values)

   // echo '<pre>';print_r($employeesMobile);die;
    if ($index % 2 !== 0) {
        $employeesMobileString .= $employeesMobile . ', '.'<br>';
    } else {
        $employeesMobileString .= $employeesMobile . '- ';
    }
}
$employeesMobileString = rtrim($employeesMobileString, ', '.'<br>');



if ($v['email'] !== null && filter_var($v['email'], FILTER_VALIDATE_EMAIL)) {

$body = "
<p style='font-family: Arial, sans-serif;'>Dear {$v['name']},</p>
<p style='font-family: Arial, sans-serif;'>The following employees have not punched in until 11:30 AM: {$v['data'][0]['CURRENT_DATE']}</p>
<table style='width: 80%; border: 1px solid #dddddd; border-collapse: collapse; font-family: Arial, sans-serif;'>
    <tr>
        <th style='background-color: #f2f2f2; border: 1px solid #dddddd; text-align: left; padding: 10px;'>Company</th>
        <th style='background-color: #f2f2f2; border: 1px solid #dddddd; text-align: left; padding: 10px;'>Department</th>
        <th style='background-color: #f2f2f2; border: 1px solid #dddddd; text-align: left; padding: 10px;'>Name</th>
        <th style='background-color: #f2f2f2; border: 1px solid #dddddd; text-align: left; padding: 10px;'>Mobile No.</th>
    </tr>";
foreach ($v['data'] as $employee) {
    $company = $employee['COMPANY_NAME'];
    $department = $employee['DEPARTMENT_NAME'];
    $name = $employee['FULL_NAME'];
    $mobile = $employee['MOBILE_NO'];

    // Add each employee's data to the table row
    $body .= "
    <tr>
        <td style='border: 1px solid #dddddd; text-align: left; padding: 10px;'>{$company}</td>
        <td style='border: 1px solid #dddddd; text-align: left; padding: 10px;'>{$department}</td>
        <td style='border: 1px solid #dddddd; text-align: left; padding: 10px;'>{$name}</td>
        <td style='border: 1px solid #dddddd; text-align: left; padding: 10px;'>{$mobile}</td>
    </tr>";
}

$body .= "</table>";




$body.="<p><a href='http://hr.neosoftware.com.np/login'>Please login to verify.</a></p>
<p>Thank you!</p>
<p>Regards,<br>HR Department</p>";

    // echo '<pre>';
    // print_r($body);
    // die;
sendEmail($mail, $subject, $v['email'], $v['name'], $body,$connection);
// mailLog($subject, $v['email'], $v['name'], );
$mail->clearAddresses();
// ...

}
}



oci_free_statement($statement);  // Clean up resources
oci_close($connection);
fclose($handle);                  // Close the file handle
