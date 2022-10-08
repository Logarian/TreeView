use #databasename#

select
	g.GovernmentCode as [��� ��]
	,case when g.GovernmentCode > 45 then g.GovernmentName else ar.AreaName end as [������������ ��]
	,s.StationCode as [��� ���]
	,s.StationName as [������� ������������ ���]
	--,case when sc.SchoolName is not NULL then sc.SchoolName else ' ' end as [������ ������������ ���]
	--,s.StationAddress as [�����]
	,case when s.StationFlags & 4 <> 0 and s.StationFlags & 8 = 0 then '���'
		  when s.StationFlags & 8 <> 0 and s.StationFlags & 4 = 0 then '���'
		  when s.StationFlags & 4 <> 0 and s.StationFlags & 8 <> 0 then '���\���'
		  end as [����� ���]
	,a.AuditoriumCode as [����� ���������]
	,a.RowsCount * a.ColsCount - pllp.Bad as [�����������]
	,a.AuditoriumName as [������������]
	,case when a.LimitPotencial = 1 then '������������������' else '�����' end as [������� ��������]
from
	rbd_Stations as s
	inner join rbd_Auditoriums as a on s.StationID = a.StationID
	inner join (
	select
	pl.AuditoriumID as [ID]
	,sum(convert(int,IsBad)) as [Bad]
	from
	rbd_Places as pl
	group by pl.AuditoriumID
	) as pllp on pllp.ID = a.AuditoriumID
	inner join rbd_Governments as g on g.GovernmentID = s.GovernmentID
	left join rbd_Areas as ar on ar.AreaCode = g.GovernmentCode
	left join rbd_Schools as sc on sc.SchoolID = s.SchoolFK
where s.DeleteType = 0 and a.DeleteType = 0
and s.StationFlags & 1 <> 0
group by
	s.StationCode
	,s.StationFlags
	,s.IsTOM
	,s.VideoControl
	,s.StationName
	,g.GovernmentCode
	,g.GovernmentName
	,ar.AreaName
	,a.AuditoriumCode
	,a.RowsCount
	,a.ColsCount
	,pllp.Bad
	,a.AuditoriumName
	,a.LimitPotencial