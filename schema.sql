create database cse3200_project;
use cse3200_project;


create table "User"(
	id int identity(1,1),
	
	creation_datetime datetime constraint df_User_creation_datetime default getDate(),
	modification_datetime datetime constraint df_User_modification_datetime default getDate(),
	
	name varchar(50) not null,
	email varchar(100) not null,
	username varchar(20) not null,
	"password" varchar(20) not null,
	"role" nvarchar(20) constraint df_User_role default 'general',
	"status" bit constraint df_User_status default 1
	
	constraint pk_User_id primary key(id),
	constraint uq_User_username unique(username),
	constraint uq_User_email unique(email),  
	constraint chk_User_email check(email like '%_@%_.%_'),
	constraint chk_User_username check(len(username) >= 4),
	constraint chk_User_password check(len("password") >= 4)
);

select * from "User";
