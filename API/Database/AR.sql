
--CREATE EXTENSION pg_trgm; CREATE EXTENSION unaccent SCHEMA;

SET SESSION AUTHORIZATION "postgres";

DROP VIEW IF EXISTS ar.v_app_1_apskritys;
DROP VIEW IF EXISTS ar.v_app_2_savivaldybes;
DROP VIEW IF EXISTS ar.v_app_3_seniunijos;
DROP VIEW IF EXISTS ar.v_app_4_gyvenvietes;
DROP VIEW IF EXISTS ar.v_app_5_gatves;
DROP VIEW IF EXISTS ar.v_app_6_adresai;
DROP VIEW IF EXISTS ar.v_app_7_patalpos;
DROP VIEW IF EXISTS ar.v_app_search;
DROP VIEW IF EXISTS ar.v_app_search_full;
DROP VIEW IF EXISTS ar.v_app_search_adr;
DROP VIEW IF EXISTS ar.v_app_detales;
DROP VIEW IF EXISTS ar.v_app_types;

GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA ar TO "_master_admin";
DO $$ DECLARE tbl RECORD; BEGIN FOR tbl IN SELECT tablename FROM pg_catalog.pg_tables WHERE schemaname = 'ar' 
LOOP EXECUTE 'ALTER TABLE ar.' || quote_ident(tbl.tablename) || ' OWNER TO "_master_admin"'; END LOOP; END $$; 

SET SESSION AUTHORIZATION "_master_admin";


DROP MATERIALIZED VIEW IF EXISTS ar.v_app_data;
CREATE MATERIALIZED VIEW ar.v_app_data AS
	WITH dt as (
		SELECT d.id, d.src, d.tipo_santrumpa trump, CASE d.src
				WHEN 'pat'::bpchar THEN concat(COALESCE(gat.vardas_k, gyv.vardas_k), ' ' || aob.nr, ' K' || aob.korpuso_nr, '-' || CASE WHEN d.src = 'pat' THEN d.vardas_k ELSE null END)
				WHEN 'aob'::bpchar THEN concat(COALESCE(gat.vardas_k, gyv.vardas_k), ' ' || aob.nr, ' K' || aob.korpuso_nr)
				ELSE concat(d.vardas_k, ' '::text || d.kita::text) END AS vardas, d.reg_data,
			CASE d.src WHEN 'pat' THEN 'patalpa' WHEN 'aob' THEN 'adresas' WHEN 'gat' THEN 'gatvė' WHEN 'gyv' THEN 'gyvenvietė' 
				WHEN 'sen' THEN 'seniūnija' WHEN 'sav' THEN 'savivaldybė' WHEN 'adm' THEN 'apskritis' END AS tipas,
			adm.adm_kodas, adm.vardas_k AS adm_vardas, adm.tipas AS adm_tipas, adm.tipo_santrumpa AS adm_trump,
			sav.sav_kodas, sav.vardas_k AS sav_vardas, sav.tipas AS sav_tipas, sav.tipo_santrumpa AS sav_trump,
			sen.sen_kodas, sen.vardas_k AS sen_vardas, sen.tipas AS sen_tipas, sen.tipo_santrumpa AS sen_trump,
			gyv.gyv_kodas, gyv.vardas_k AS gyv_vardas, gyv.tipas AS gyv_tipas, gyv.tipo_santrumpa AS gyv_trump,
				CASE WHEN d.src = 'gyv' THEN d.kita ELSE gyv.vardas END AS gyv_pavad,
			gat.gat_kodas, gat.vardas_k AS gat_vardas, gat.tipas AS gat_tipas, gat.tipo_santrumpa AS gat_trump,
			aob.aob_kodas, aob.nr AS aob_nr, aob.korpuso_nr AS aob_korpusas,
				CASE WHEN d.src = 'pat' THEN d.vardas_k ELSE null END as aob_patalpa, aob.pasto_kodas AS aob_post
		FROM ar.data d
			LEFT JOIN ar.adr_1_apskritys adm ON d.adm_kodas = adm.adm_kodas or (d.src='adm' and d.id=adm.adm_kodas)
			LEFT JOIN ar.adr_2_savivaldybes sav ON d.sav_kodas = sav.sav_kodas or (d.src='sav' and d.id=sav.sav_kodas)
			LEFT JOIN ar.adr_3_seniunijos sen ON d.sen_kodas = sen.sen_kodas or (d.src='sen' and d.id=sen.sen_kodas)
			LEFT JOIN ar.adr_4_gyvenvietes gyv ON d.gyv_kodas = gyv.gyv_kodas or (d.src='gyv' and d.id=gyv.gyv_kodas)
			LEFT JOIN ar.adr_5_gatves gat ON d.gat_kodas = gat.gat_kodas or (d.src='gat' and d.id=gat.gat_kodas)
			LEFT JOIN ar.adr_6_adresai aob ON d.aob_kodas = aob.aob_kodas or (d.src='aob' and d.id=aob.aob_kodas)
		WHERE d.aktyvus = true
	), 
	admc as (SELECT adm_kodas id, count(*)::int adm_cnt FROM ar.adr_2_savivaldybes WHERE adm_kodas is not null GROUP By adm_kodas),
	savc as (SELECT sav_kodas id, count(*)::int sav_cnt FROM ar.adr_3_seniunijos WHERE sav_kodas is not null GROUP By sav_kodas),
	senc as (SELECT sen_kodas id, count(*)::int sen_cnt FROM ar.adr_4_gyvenvietes WHERE sen_kodas is not null GROUP By sen_kodas),
	gyvc as (SELECT gyv_kodas id, count(*)::int gyv_cnt FROM ar.adr_5_gatves WHERE gyv_kodas is not null GROUP By gyv_kodas),
	gatc as (SELECT gat_kodas id, count(*)::int gat_cnt FROM ar.adr_6_adresai WHERE gat_kodas is not null GROUP By gat_kodas),
	aobc as (SELECT aob_kodas id, count(*)::int aob_cnt FROM ar.adr_7_patalpos WHERE aob_kodas is not null GROUP By aob_kodas),	
	savm as (SELECT sav_kodas id, count(*)::int sav_mis FROM ar.adr_4_gyvenvietes WHERE sen_kodas is null GROUP By sav_kodas),
	gyvm as (SELECT gyv_kodas id, count(*)::int gyv_mis FROM ar.adr_6_adresai WHERE gat_kodas is null GROUP By gyv_kodas),
	admg as (SELECT aps_kodas::int id, ar.ST_Area(geom)::bigint plotas FROM ar.geo_1_apskritys),
	savg as (SELECT sav_kodas::int id, ar.ST_Area(geom)::bigint plotas FROM ar.geo_2_savivaldybes),
	seng as (SELECT sen_kodas::int id, ar.ST_Area(geom)::bigint plotas FROM ar.geo_3_seniunijos),
	gyvg as (SELECT gyv_kodas::int id, ar.ST_Area(geom)::bigint plotas FROM ar.geo_4_gyvenvietes),
	gatg as (SELECT gat_kodas::int id, ar.ST_Length(geom)::bigint ilgis FROM ar.geo_5_gatves),
	geo  as (SELECT DISTINCT aob_kodas id, ARRAY[x_koord, y_koord] aob_lks, ARRAY[e_koord, n_koord] aob_wgs FROM ar.geo_6_adresai),
	dm as (
		SELECT dt.id, src,
		CASE src
				WHEN 'pat' THEN CONCAT(COALESCE(gat_vardas || ' ' || gat_trump,gyv_vardas || ' ' || gyv_trump), ' ' || aob_nr, ' K' || aob_korpusas, '-' || aob_patalpa )
				WHEN 'aob' THEN CONCAT(COALESCE(gat_vardas || ' ' || gat_trump,gyv_vardas || ' ' || gyv_trump), ' ' || aob_nr, ' K' || aob_korpusas )
				WHEN 'gat' THEN gat_vardas || ' ' || gat_tipas WHEN 'gyv' THEN gyv_vardas || ' ' || gyv_tipas WHEN 'sen' THEN sen_vardas || ' ' || sen_tipas
				WHEN 'sav' THEN sav_vardas || ' ' || sav_tipas WHEN 'adm' THEN adm_vardas || ' ' || adm_tipas END AS pavad,
			CASE 
				WHEN src in ('pat','aob','gat','gyv') THEN CONCAT(REPLACE(REPLACE(sav_vardas,' miesto',' m.'),' rajono',' r.') || ' ' || sav_trump, ', ' || 
					REPLACE(REPLACE(sen_vardas,' miesto',' m.'),' kaimiškoji',' k.') || ' ' || sen_trump, CASE WHEN gat_kodas is NOT NULL THEN ', ' || gyv_pavad END )
				WHEN src = 'sen' THEN CONCAT(adm_vardas || ' ' || adm_trump , ', ' || REPLACE(REPLACE(sav_vardas,' miesto',' m.'),' rajono',' r.') || ' ' || sav_trump )
				WHEN src = 'sav' THEN adm_vardas || ' ' || adm_trump END AS vietove,
			tipas,reg_data,
			adm_kodas,adm_vardas,adm_tipas,adm_trump,COALESCE(adm_cnt,0) adm_cnt,
			sav_kodas,sav_vardas,sav_tipas,sav_trump,COALESCE(sav_cnt,0) sav_cnt, sav_mis,
			sen_kodas,sen_vardas,sen_tipas,sen_trump,COALESCE(sen_cnt,0) sen_cnt,
			gyv_kodas,gyv_vardas,gyv_tipas,gyv_trump,gyv_pavad,COALESCE(gyv_cnt,0) gyv_cnt, gyv_mis,
			gat_kodas,gat_vardas,gat_tipas,gat_trump,COALESCE(gat_cnt,0) gat_cnt,
			aob_kodas,aob_nr,aob_korpusas,aob_patalpa,COALESCE(aob_cnt,0) aob_cnt,aob_post,aob_lks,aob_wgs,
			src='pat' as aob_pat, COALESCE(gatg.ilgis,gyvg.plotas,seng.plotas,savg.plotas,admg.plotas) as dydis,
			array_to_string(array(select distinct unnest(string_to_array(ar.unaccent(lower(
				REPLACE(REPLACE(CONCAT(adm_vardas,' ' || sav_vardas,' ' || sen_vardas,' ' || gyv_vardas,' ' || gyv_pavad,' ' || gat_vardas, CONCAT(' ' || aob_nr, ' K' || aob_korpusas, '-' || aob_patalpa)),' miesto',''),' rajono','')
			)), ' '))), ' ') as srh2, ar.unaccent(lower(vardas)) as srh1
		FROM dt 
			LEFT JOIN admc on (adm_kodas=admc.id) LEFT JOIN admg on (adm_kodas=admg.id)
			LEFT JOIN savc on (sav_kodas=savc.id) LEFT JOIN savg on (sav_kodas=savg.id)
			LEFT JOIN senc on (sen_kodas=senc.id) LEFT JOIN seng on (sen_kodas=seng.id)
			LEFT JOIN gyvc on (gyv_kodas=gyvc.id) LEFT JOIN gyvg on (gyv_kodas=gyvg.id)
			LEFT JOIN gatc on (gat_kodas=gatc.id) LEFT JOIN gatg on (gat_kodas=gatg.id)
			LEFT JOIN aobc on (aob_kodas=aobc.id) LEFT JOIN savm on (sav_kodas=savm.id) 
			LEFT JOIN gyvm on (gyv_kodas=gyvm.id) LEFT JOIN geo  on (aob_kodas=geo.id)
	)
	
	SELECT *,
		CASE src
			WHEN 'gyv' THEN
				CASE gyv_trump
					WHEN 'm.' THEN dydis*0.001
					WHEN 'mstl.' THEN dydis*0.0001
					ELSE dydis*0.000001
				END
			ELSE dydis*0.000001
		END as sort
	FROM dm;

CREATE INDEX ar_v_app_data_id_idx on ar.v_app_data (id);
CREATE INDEX ar_v_app_data_src_idx on ar.v_app_data (src);
CREATE INDEX ar_v_app_data_sort_idx on ar.v_app_data (sort);
CREATE INDEX ar_v_app_data_adm_idx on ar.v_app_data (adm_kodas);
CREATE INDEX ar_v_app_data_sav_idx on ar.v_app_data (sav_kodas);
CREATE INDEX ar_v_app_data_sen_idx on ar.v_app_data (sen_kodas);
CREATE INDEX ar_v_app_data_gyv_idx on ar.v_app_data (gyv_kodas);
CREATE INDEX ar_v_app_data_gat_idx on ar.v_app_data (gat_kodas);
CREATE INDEX ar_v_app_data_aob_idx on ar.v_app_data (aob_kodas);
CREATE INDEX ar_v_app_data_post_idx on ar.v_app_data (aob_post);

CREATE INDEX ar_v_app_data_srh1_idx ON ar.v_app_data USING gin(srh1 gin_trgm_ops);
CREATE INDEX ar_v_app_data_srh2_idx ON ar.v_app_data USING gin(srh2 gin_trgm_ops);

CREATE VIEW ar.v_app_1_apskritys AS SELECT id "ID", pavad "Pavad", vietove "Vietove", adm_vardas "Vardas", adm_tipas "Tipas", adm_trump "Trump", adm_cnt "Chc", reg_data "RegData", srh1 as search FROM ar.v_app_data WHERE src='adm';
CREATE VIEW ar.v_app_2_savivaldybes AS SELECT id "ID", pavad "Pavad", vietove "Vietove", adm_kodas "Adm", sav_vardas "Vardas", sav_tipas "Tipas", sav_trump "Trump", sav_cnt "Chc", sav_mis "Chm", reg_data "RegData", srh1 as search FROM ar.v_app_data WHERE src='sav';
CREATE VIEW ar.v_app_3_seniunijos AS SELECT id "ID", pavad "Pavad", vietove "Vietove", adm_kodas "Adm", sav_kodas "Sav", sen_vardas "Vardas", sen_tipas "Tipas", sen_trump "Trump", sen_cnt "Chc", reg_data "RegData", srh1 as search FROM ar.v_app_data WHERE src='sen';
CREATE VIEW ar.v_app_4_gyvenvietes AS SELECT id "ID", pavad "Pavad", vietove "Vietove", adm_kodas "Adm", sav_kodas "Sav", sen_kodas "Sen", gyv_vardas "Vardas", gyv_tipas "Tipas", gyv_trump "Trump", gyv_cnt "Chc", gyv_mis "Chm", reg_data "RegData", srh1 as search FROM ar.v_app_data WHERE src='gyv';
CREATE VIEW ar.v_app_5_gatves AS SELECT id "ID", pavad "Pavad", vietove "Vietove", adm_kodas "Adm", sav_kodas "Sav", sen_kodas "Sen", gyv_kodas "Gyv", gat_vardas "Vardas", gat_tipas "Tipas", gat_trump "Trump", gat_cnt "Chc", reg_data "RegData", srh1 as search FROM ar.v_app_data WHERE src='gat';
CREATE VIEW ar.v_app_6_adresai AS SELECT id "ID", pavad "Pavad", vietove "Vietove", adm_kodas "Adm", sav_kodas "Sav", sen_kodas "Sen", gyv_kodas "Gyv", gat_kodas "Gat", aob_nr "Nr", aob_korpusas as "Korp", aob_post as "Post", aob_cnt "Chc", reg_data "RegData", srh1 as search FROM ar.v_app_data WHERE src='aob';
CREATE VIEW ar.v_app_7_patalpos AS SELECT id "ID", pavad "Pavad", vietove "Vietove", adm_kodas "Adm", sav_kodas "Sav", sen_kodas "Sen", gyv_kodas "Gyv", gat_kodas "Gat", aob_kodas "Aob", aob_patalpa "Pat", aob_post as "Post", reg_data "RegData", srh1 as search FROM ar.v_app_data WHERE src='pat';

CREATE VIEW ar.v_app_types AS SELECT DISTINCT tipas FROM (SELECT data.tipas FROM ar.data UNION SELECT data.tipo_santrumpa FROM ar.data) WHERE tipas IS NOT NULL;
CREATE VIEW ar.v_app_search AS SELECT id "ID",pavad "Pavad",vietove "Vietove",tipas "Tipas",src "Src",adm_kodas "Adm",sav_kodas "Sav",sen_kodas "Sen",gyv_kodas "Gyv",gat_kodas "Gat",aob_kodas "Aob",srh1 search,sort FROM ar.v_app_data; 
CREATE VIEW ar.v_app_search_full AS SELECT id "ID",pavad "Pavad",vietove "Vietove",tipas "Tipas",src "Src",adm_kodas "Adm",sav_kodas "Sav",sen_kodas "Sen",gyv_kodas "Gyv",gat_kodas "Gat",aob_kodas "Aob",srh2 search,sort FROM ar.v_app_data;
CREATE VIEW ar.v_app_search_adr AS SELECT id "ID",pavad "Pavad",vietove "Vietove",tipas "Tipas", adm_kodas "Adm",sav_kodas "Sav",sen_kodas "Sen",gyv_kodas "Gyv",gat_kodas "Gat",aob_kodas "Aob",
	srh1 search,sort FROM ar.v_app_data WHERE src in ('aob','pat');

CREATE VIEW ar.v_app_detales AS SELECT id, src, pavad, vietove, tipas, reg_data, adm_kodas, adm_vardas, adm_tipas, adm_trump, adm_cnt,
	sav_kodas, sav_vardas, sav_tipas, sav_trump, sav_cnt, sav_mis, sen_kodas, sen_vardas, sen_tipas, sen_trump, sen_cnt,
	gyv_kodas, gyv_vardas, gyv_pavad, gyv_tipas, gyv_trump, gyv_cnt, gyv_mis, gat_kodas, gat_vardas, gat_tipas, gat_trump, gat_cnt, 
	aob_kodas, aob_cnt, aob_nr, aob_korpusas, aob_patalpa, aob_post, aob_lks, aob_wgs, dydis FROM ar.v_app_data;

CREATE OR REPLACE FUNCTION ar.geo_adr_find(x_point float, y_point float) RETURNS TABLE(aob integer, gyv integer, gat integer, x integer, y integer) LANGUAGE 'plpgsql' AS $BODY$ BEGIN RETURN QUERY SELECT aob_kodas, gyv_kodas, gat_kodas, x_koord, y_koord FROM ar.geo_6_adresai WHERE ar.ST_Contains(geom, ar.ST_Point(x_point,y_point,3346)) LIMIT 1; END; $BODY$;
CREATE OR REPLACE FUNCTION ar.geo_adr_near(x_point float, y_point float, distance integer DEFAULT 50, reslimit int DEFAULT 5) RETURNS TABLE(dist float, aob integer, gyv integer, gat integer, x integer, y integer) LANGUAGE 'plpgsql' AS $BODY$ DECLARE fltr int; BEGIN SELECT gyv_kodas INTO fltr FROM ar.geo_4_gyvenvietes WHERE ar.ST_Contains(geom, ar.ST_Point(x_point, y_point, 3346)); RETURN QUERY SELECT ROUND(t.dist::numeric,2)::float dist,aob_kodas,gyv_kodas,gat_kodas,x_koord,y_koord FROM (SELECT ar.ST_Distance(geom, ar.ST_Point(x_point, y_point, 3346)) dist, aob_kodas, gyv_kodas, gat_kodas, x_koord, y_koord FROM ar.geo_6_adresai WHERE gyv_kodas=fltr ORDER BY dist ASC LIMIT reslimit ) t WHERE t.dist<=distance; END; $BODY$;


-- TODO: Normaliai teises sudėlioti
GRANT SELECT ON ALL TABLES IN SCHEMA ar TO "registrai_app";
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA ar TO "registrai_app";

/*

-- pridedamas REFRESH MATERIALIZED VIEW ar.v_app_data;

CREATE OR REPLACE FUNCTION ar.data_load() RETURNS TABLE(id bigint, source varchar, "table" varchar, status bool, items bigint, duration int, inserted int, updated int, deleted int) LANGUAGE 'plpgsql' SECURITY DEFINER AS $BODY$ 
   DECLARE imp timestamp(3)=timezone('utc'::text, now()); strt timestamp; ins int; upd int; del int; lst varchar(255)[][]; BEGIN
   strt:=clock_timestamp(); SELECT e.ins,e.upd,e.del,clock_timestamp() FROM ar.load_1_apskritys() e INTO ins,upd,del; INSERT INTO ar.log_import (log_date,log_type,log_source,log_table,log_status,log_items,log_duration,log_data_ins,log_data_upd,log_data_del) VALUES (imp,'load','apskritys','adr_1_apskritys',true,ins+upd+del,EXTRACT(milliseconds FROM age(clock_timestamp(),strt)),ins,upd,del);
   strt:=clock_timestamp(); SELECT e.ins,e.upd,e.del,clock_timestamp() FROM ar.load_2_savivaldybes() e INTO ins,upd,del; INSERT INTO ar.log_import (log_date,log_type,log_source,log_table,log_status,log_items,log_duration,log_data_ins,log_data_upd,log_data_del) VALUES (imp,'load','savivaldybes','adr_2_savivaldybes',true,ins+upd+del,EXTRACT(milliseconds FROM age(clock_timestamp(),strt)),ins,upd,del);
   strt:=clock_timestamp(); SELECT e.ins,e.upd,e.del,clock_timestamp() FROM ar.load_3_seniunijos() e INTO ins,upd,del; INSERT INTO ar.log_import (log_date,log_type,log_source,log_table,log_status,log_items,log_duration,log_data_ins,log_data_upd,log_data_del) VALUES (imp,'load','seniunijos','adr_3_seniunijos',true,ins+upd+del,EXTRACT(milliseconds FROM age(clock_timestamp(),strt)),ins,upd,del);
   strt:=clock_timestamp(); SELECT e.ins,e.upd,e.del,clock_timestamp() FROM ar.load_4_gyvenvietes() e INTO ins,upd,del; INSERT INTO ar.log_import (log_date,log_type,log_source,log_table,log_status,log_items,log_duration,log_data_ins,log_data_upd,log_data_del) VALUES (imp,'load','gyvenvietes','adr_4_gyvenvietes',true,ins+upd+del,EXTRACT(milliseconds FROM age(clock_timestamp(),strt)),ins,upd,del);
   strt:=clock_timestamp(); SELECT e.ins,e.upd,e.del,clock_timestamp() FROM ar.load_5_gatves() e INTO ins,upd,del; INSERT INTO ar.log_import (log_date,log_type,log_source,log_table,log_status,log_items,log_duration,log_data_ins,log_data_upd,log_data_del) VALUES (imp,'load','gatves','adr_5_gatves',true,ins+upd+del,EXTRACT(milliseconds FROM age(clock_timestamp(),strt)),ins,upd,del);
   strt:=clock_timestamp(); SELECT e.ins,e.upd,e.del,clock_timestamp() FROM ar.load_6_adresai() e INTO ins,upd,del; INSERT INTO ar.log_import (log_date,log_type,log_source,log_table,log_status,log_items,log_duration,log_data_ins,log_data_upd,log_data_del) VALUES (imp,'load','adresai','adr_6_adresai',true,ins+upd+del,EXTRACT(milliseconds FROM age(clock_timestamp(),strt)),ins,upd,del);
   strt:=clock_timestamp(); SELECT e.ins,e.upd,e.del,clock_timestamp() FROM ar.load_7_patalpos() e INTO ins,upd,del; INSERT INTO ar.log_import (log_date,log_type,log_source,log_table,log_status,log_items,log_duration,log_data_ins,log_data_upd,log_data_del) VALUES (imp,'load','patalpos','adr_7_patalpos',true,ins+upd+del,EXTRACT(milliseconds FROM age(clock_timestamp(),strt)),ins,upd,del);
   strt:=clock_timestamp(); SELECT e.ins,e.upd,e.del,clock_timestamp() FROM ar.load_8_data() e INTO ins,upd,del; INSERT INTO ar.log_import (log_date,log_type,log_source,log_table,log_status,log_items,log_duration,log_data_ins,log_data_upd,log_data_del) VALUES (imp,'load','data','data',true,ins+upd+del,EXTRACT(milliseconds FROM age(clock_timestamp(),strt)),ins,upd,del);
   REFRESH MATERIALIZED VIEW ar.v_app_data;
   RETURN QUERY SELECT log_id, log_source, log_table, log_status, log_items, log_duration, log_data_ins,log_data_upd,log_data_del FROM ar.log_import WHERE log_date = imp; END; 
$BODY$;

*/