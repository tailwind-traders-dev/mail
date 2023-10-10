drop schema if exists mail cascade;
create schema mail;
set search_path=mail;

create table contacts(
  id serial primary key,
  email text not null unique,
  subscribed boolean not null default true,
  name text 
);

create table groups(
  id serial not null primary key,
  slug text not null unique,
  name text,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table tags(
  id serial not null primary key,
  slug text not null unique,
  name text,
  desctiption text,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table segments(
  contact_id int not null references contacts(id),
  group_id int not null references groups(id),
  primary key (contact_id, group_id)
);

create table contacts_tags(
  contact_id int not null references contacts(id),
  tag_id int not null references tags(id),
  primary key (contact_id, tag_id)
);

create table sequences(
  id serial not null primary key,
  slug text not null unique,
  name text,
  description text,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

-- templates
create table emails(
  id serial not null primary key,
  sequence_id int references sequences(id),
  slug text not null unique,
  subject text not null,
  preview text,
  delay_hours int not null default 0,
  markdown text,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

--actual emails sent
create table messages(
  id serial not null primary key,
  email_id int not null references emails(id),
  send_to text not null,
  send_from text not null,
  subject text not null,
  html text not null,
  send_at timestamptz,
  sent_at timestamptz not null default now(),
  receipt jsonb,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);


