use #databasename#
select
	g.GovernmentCode as [Код МСУ]
	,ss.StationCode as [Код ППЭ]
	,ss.StationName as [Краткое наименование ППЭ]
	--,case when s.SchoolName is not NULL then s.SchoolName else ' ' end as [Полное наименование ППЭ]
	--,ss.StationAddress as [Адрес]
	,case when ss.StationFlags & 4 <> 0 and ss.StationFlags & 8 = 0 then 'ЕГЭ'
		  when ss.StationFlags & 8 <> 0 and ss.StationFlags & 4 = 0 then 'ГВЭ'
		  when ss.StationFlags & 4 <> 0 and ss.StationFlags & 8 <> 0 then 'ЕГЭ\ГВЭ'
		  end as [Форма]

	,case when ss.IsTOM = 1 then '+' 
		  when ss.IsTom = 0 then '-' end as [ТОМ]

	,case when ss.StationFlags & 64 <> 0 then '+' else '-' end as [Отдалённый]
	,case when ss.StationFlags & 1 <> 0 then '+' else '-' end as [На дому]
	,case when ss.StationFlags & 512 <> 0 then '+' else '-' end as [ППЭ в учреждении УФСИН]
	,case when ss.VideoControl = 1 then '+' else '-' end as [Онлайн видеонаблюдение]
	,case when sp.PropertyValue is NULL or sp.PropertyValue = 0 then 'On' else 'Off' end as [Штаб]
	,case when ss.StationFlags & 32 <> 0 then '+' else '-' end as [Печать КИМ]
	,case when ss.StationFlags & 256 <> 0 then '+' else '-' end as [Сканирование в ППЭ]
	,count(distinct au.AuditoriumID) as [Всего аудиторий]
	--,count(distinct case when au.LimitPotencial = 0 then au.AuditoriumID else null end) as [Количество аудиторий обычных]
	--,count(distinct case when au.LimitPotencial = 1 then au.AuditoriumID else null end) as [Количество аудиторий специализированных]
	--,count(distinct case when au.LimitPotencial = 1 and au.VideoControl = 1 then au.AuditoriumID else null end) as [Количество аудиторий из специализированных с онлайн]
	,count(distinct case when au.VideoControl = 1 then au.AuditoriumID else null end) as [Всего аудиторий онлайн]
	,count(distinct case when au.VideoControl = 0 then au.AuditoriumID else null end) as [Всего аудиторий офлайн]
	,count(distinct case when au.ExamForm & 8 <> 0 then au.AuditoriumID else null end) as [ГВЭ]
	,count(distinct case when au.ExamForm & 1 <> 0 then au.AuditoriumID else null end) as [КЕГЭ]
	,count(distinct case when au.ExamForm & 2 <> 0 then au.AuditoriumID else null end) as [Проведения]
	,count(distinct case when au.ExamForm & 4 <> 0 then au.AuditoriumID else null end) as [Подготовки]
from
	rbd_Stations as ss
	inner join rbd_Governments as g on g.GovernmentID = ss.GovernmentID
	left join rbd_Auditoriums as au on ss.StationID = au.StationID
	left join rbd_Schools as s on s.SchoolID = ss.SchoolFK
	left join rbd_StationProperties as sp on sp.StationID = ss.StationID and sp.PropertyType = 1 --офлайн\онлайн
where ss.DeleteType = 0 and (au.DeleteType = 0 or au.DeleteType is NULL)

group by
	g.GovernmentCode
	,ss.StationCode
	,ss.StationFlags
	,ss.IsTOM
	,ss.VideoControl
	,ss.StationName
	,s.SchoolName
	,ss.StationAddress
	,sp.PropertyValue