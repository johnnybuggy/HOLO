attach database "test.db" as dt;
--attach database "test_winners.db" as tw;

delete from main where id in (select id from (select id, count(statvalue) as t from stats group by id having t < 322));
delete from stats where id in (select id from (select id, count(statvalue) as t from stats group by id having t < 322));
--delete from tw.score_winners;
