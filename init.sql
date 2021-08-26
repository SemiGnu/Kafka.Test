create database mystore;
use mystore;
create table products (id int unsigned auto_increment primary key, name varchar(50), price int, creation_time datetime default current_timestamp, modification_time datetime on update current_timestamp);
insert into products(id, name, price) values(1, "Red T-Shirt", 12);

create database my_other_store;
use my_other_store;
CREATE TABLE IF NOT EXISTS `my_other_store`.`Products` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` LONGTEXT NULL DEFAULT NULL,
  `Price` INT(11) NOT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8mb4;
insert into Products(Id, Name, Price) values(1, "Red T-Shirt", 12);

CREATE USER 'debezium' IDENTIFIED BY 'dbz';
GRANT SELECT, RELOAD, SHOW DATABASES, REPLICATION SLAVE, REPLICATION CLIENT ON *.* TO 'debezium'@'%';

