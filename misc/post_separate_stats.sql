.echo ON
attach database "test.db" as dt;

--attach database "db\b_mean_fft1.db" as b01; create table b01.stats as select * from dt.stats where statname like "b_mean_fft1_%";
--attach database "db\b_median_fft1.db" as b02; create table b02.stats as select * from dt.stats where statname like "b_median_fft1_%";
--attach database "db\b_stdev_fft1.db" as b03; create table b03.stats as select * from dt.stats where statname like "b_stdev_fft1_%";
--attach database "db\b_skewn_fft1.db" as b04; create table b04.stats as select * from dt.stats where statname like "b_skewn_fft1_%";
--attach database "db\b_kurto_fft1.db" as b05; create table b05.stats as select * from dt.stats where statname like "b_kurto_fft1_%";

--attach database "db\sq_mean_fft1.db" as b06; create table b06.stats as select * from dt.stats where statname like "sq_mean_fft1_%";
--attach database "db\sq_median_fft1.db" as b07; create table b07.stats as select * from dt.stats where statname like "sq_median_fft1_%";
--attach database "db\sq_stdev_fft1.db" as b08; create table b08.stats as select * from dt.stats where statname like "sq_stdev_fft1_%";
--attach database "db\sq_skewn_fft1.db" as b09; create table b09.stats as select * from dt.stats where statname like "sq_skewn_fft1_%";
attach database "db\sq_kurto_fft1.db" as b10; create table b10.stats as select * from dt.stats where statname like "sq_kurto_fft1_%";

attach database "db\log_mean_fft1.db" as b11; create table b11.stats as select * from dt.stats where statname like "log_mean_fft1_%";
attach database "db\log_median_fft1.db" as b12; create table b12.stats as select * from dt.stats where statname like "log_median_fft1_%";
attach database "db\log_stdev_fft1.db" as b13; create table b13.stats as select * from dt.stats where statname like "log_stdev_fft1_%";
attach database "db\log_kurto_fft1.db" as b14; create table b14.stats as select * from dt.stats where statname like "log_kurto_fft1_%";