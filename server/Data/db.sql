drop schema mail cascade;
create schema mail;
set search_path="mail";

create table contact(
  id serial primary key,
  email text not null unique

);