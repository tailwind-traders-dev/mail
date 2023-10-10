drop schema mail cascade;
create schema mail;
set search_path="mail";

create table contacts(
  id serial primary key,
  email text not null unique
);