attach database "test.db" as dt;

DROP INDEX IF EXISTS dt.idx_statname;
DROP INDEX IF EXISTS dt.idx_id_statname;
CREATE INDEX IF NOT EXISTS dt.idx3 ON stats (id, statname, statvalue);
VACUUM;

.output "calc_feat1.csv"

create table if not exists dt.feature(statname string, feature string, value float);
delete from dt.feature;

insert into feature
	select statname, "max", max(statvalue)
	from stats
	group by statname
;

insert into feature
	select statname, "min", min(statvalue)
	from stats
	group by statname
;

insert into feature
	select statname, "max_min", max(statvalue)-min(statvalue)
	from stats
	group by statname
;

insert into feature
	select statname, "mean", avg(statvalue)
	from stats
	group by statname
;

insert into feature
	select b.statname, "disp", avg((b.statvalue - c.value)*(b.statvalue - c.value))
	from dt.stats b, dt.feature c
	where b.statname = c.statname and c.feature = "mean"
	group by b.statname
;

select * from feature
order by feature, statname;

select statname, max(statvalue), min(statvalue), avg(statvalue), max(statvalue)-min(statvalue), (avg(statvalue)-min(statvalue))/(max(statvalue)-min(statvalue))
from stats
group by statname
order by statname
;

DROP INDEX IF EXISTS dt.idx_statname_feat;
DROP INDEX IF EXISTS dt.idx_statname;
CREATE INDEX IF NOT EXISTS dt.idx_statname_feat ON feature (statname, feature, value);
CREATE INDEX IF NOT EXISTS dt.idx_statname ON feature (statname, value);

drop table if exists dt.about;
create table if not exists dt.about as
select "count_statname" as name, count(distinct statname) as value from feature;

insert into dt.about
select distinct "name" as name, statname as value from feature;

select * from about;

attach database "test_winners.db" as dw;

create table if not exists dt.score_winners (statname string, score int, win_type string);
create table if not exists dw.winners (id_tgt string, id_winner string, win_type string);
delete from dw.winners;

VACUUM;