CREATE TABLE IF NOT EXISTS competences
(id INT UNSIGNED NOT NULL AUTO_INCREMENT,
nom VARCHAR(100) NOT NULL,
commentaire VARCHAR(250),
PRIMARY KEY(id));

create table if not exists technos
(id int unsigned not null auto_increment,
nom varchar(100) not null,
commentaire varchar(250),
primary key(id));

CREATE TABLE IF NOT EXISTS societes
(id int unsigned not null auto_increment,
nom varchar(100) not null,
commentaire varchar(250),
primary key(id));

create table if not exists consultants
(id int unsigned not null auto_increment,
nom varchar(100) not null,
prenom varchar(100) not null,
photo mediumblob,
minicv mediumblob,
primary key(id));

CREATE TABLE IF NOT EXISTS consultantcompetences
(idconsultant INT UNSIGNED NOT NULL,
idcompetence INT UNSIGNED NOT NULL,
PRIMARY KEY(idconsultant, idcompetence),
foreign key(idconsultant) references consultants (id) on delete cascade,
foreign key(idcompetence) references competences (id) on delete cascade);

CREATE TABLE IF NOT EXISTS consultanttechnos
(idconsultant INT unsigned NOT NULL,
idtechno INT unsigned NOT NULL,
primary key(idconsultant, idtechno),
foreign key(idconsultant) references consultants (id) on delete cascade,
foreign key(idtechno) references technos (id) on delete cascade);

create table if not exists missions
(id int unsigned not null auto_increment,
societeid int unsigned not null,
consultantid int unsigned not null,
datedebut datetime not null,
datefin datetime,
commentaire varchar(250),
primary key(id),
foreign key(consultantid) references consultants (id),
foreign key(societeid) references societes (id));