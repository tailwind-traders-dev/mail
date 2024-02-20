test: db seed
	dotnet test

db:
	psql tailwind < ../db/db.sql --quiet

seed: db
	psql tailwind < ../db/seed.sql --quiet

mailhog:
	docker run -d -p 1025:1025 -p 8025:8025 mailhog/mailhog


.phony: test db seed change_1 mailhog

#pg_dump -d tailwind --no-owner --no-privileges --no-password --table "mail.contacts" --inserts -f ./dump.sql
#have to find/replace using ^\d*.\t