attach database "test_winners.db" as dw;
attach database "test.db" as dt;

create table if not exists dt.score_winners (statname string, score int, win_type string);
create table if not exists dw.winners (id_tgt string, id_winner string, win_type string);

.output "score.txt"

delete from dt.score_winners;

insert into dt.score_winners
select statname, sum(score)*sum(score)*sum(score) as score, "all" as win_type from
(
select c.statname as statname, 1 as score, d.win_type as win_type
from dt.stats a, dt.stats b, dt.feature c, dw.winners d
where
a.statname = b.statname and
a.statname = c.statname and
a.id = d.id_tgt and
b.id = d.id_winner and
c.feature = "max_min" and
d.win_type not in ("WTF") and
((a.statvalue - b.statvalue)/c.value < 0.02 and
(b.statvalue - a.statvalue)/c.value < 0.02)
                                         
UNION ALL

select c.statname as statname, -1 as score, d.win_type as win_type
from dt.stats a, dt.stats b, dt.feature c, dw.winners d
where
a.statname = b.statname and
a.statname = c.statname and
a.id = d.id_tgt and
b.id = d.id_winner and
c.feature = "max_min" and
d.win_type = "WTF" and
((a.statvalue - b.statvalue)/c.value > 0.4 or
(b.statvalue - a.statvalue)/c.value > 0.4)

UNION ALL

select distinct c.statname as statname, 1 as score, "all" as win_type
from dt.feature c

)
group by statname
;

CREATE INDEX IF NOT EXISTS dt.iwin ON score_winners (statname, score);

select statname, score from dt.score_winners;