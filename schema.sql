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

create table Content(
	id int identity(1, 1),
	
	creator_id int,
	creation_datetime datetime constraint df_content_creation_datetime default getDate(),
	
	modifier_id int,
	modification_datetime datetime constraint df_content_modification_datetime default getDate(),
	
	title varchar(50) not null,
	details text,
	url varchar(500) not null,
	alternative_url varchar(1000),
	"type" varchar(20),
	privacy varchar(20) constraint df_content_privacy default 'private',
	"status" bit constraint df_content_status default 1,
	
	constraint pk_content_id primary key(id),
	constraint fk_content_creator_id foreign key(creator_id) references "user"(id),
	constraint fk_content_modifier_id foreign key(modifier_id) references "user"(id),
	
	constraint uq_content_url_creator_id unique(url, creator_id),
	constraint uq_content_title_creator_id unique(title, creator_id)
);

select * from Content;

