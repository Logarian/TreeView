use #databasename#
select
	g.GovernmentCode as [��� ���]
	,ss.StationCode as [��� ���]
	,ss.StationName as [������� ������������ ���]
	--,case when s.SchoolName is not NULL then s.SchoolName else ' ' end as [������ ������������ ���]
	--,ss.StationAddress as [�����]
	,case when ss.StationFlags & 4 <> 0 and ss.StationFlags & 8 = 0 then '���'
		  when ss.StationFlags & 8 <> 0 and ss.StationFlags & 4 = 0 then '���'
		  when ss.StationFlags & 4 <> 0 and ss.StationFlags & 8 <> 0 then '���\���'
		  end as [�����]

	,case when ss.IsTOM = 1 then '+' 
		  when ss.IsTom = 0 then '-' end as [���]

	,case when ss.StationFlags & 64 <> 0 then '+' else '-' end as [���������]
	,case when ss.StationFlags & 1 <> 0 then '+' else '-' end as [�� ����]
	,case when ss.StationFlags & 512 <> 0 then '+' else '-' end as [��� � ���������� �����]
	,case when ss.VideoControl = 1 then '+' else '-' end as [������ ���������������]
	,case when sp.PropertyValue is NULL or sp.PropertyValue = 0 then 'On' else 'Off' end as [����]
	,case when ss.StationFlags & 32 <> 0 then '+' else '-' end as [������ ���]
	,case when ss.StationFlags & 256 <> 0 then '+' else '-' end as [������������ � ���]
	,count(distinct au.AuditoriumID) as [����� ���������]
	--,count(distinct case when au.LimitPotencial = 0 then au.AuditoriumID else null end) as [���������� ��������� �������]
	--,count(distinct case when au.LimitPotencial = 1 then au.AuditoriumID else null end) as [���������� ��������� ������������������]
	--,count(distinct case when au.LimitPotencial = 1 and au.VideoControl = 1 then au.AuditoriumID else null end) as [���������� ��������� �� ������������������ � ������]
	,count(distinct case when au.VideoControl = 1 then au.AuditoriumID else null end) as [����� ��������� ������]
	,count(distinct case when au.VideoControl = 0 then au.AuditoriumID else null end) as [����� ��������� ������]
	,count(distinct case when au.ExamForm & 8 <> 0 then au.AuditoriumID else null end) as [���]
	,count(distinct case when au.ExamForm & 1 <> 0 then au.AuditoriumID else null end) as [����]
	,count(distinct case when au.ExamForm & 2 <> 0 then au.AuditoriumID else null end) as [����������]
	,count(distinct case when au.ExamForm & 4 <> 0 then au.AuditoriumID else null end) as [����������]
from
	rbd_Stations as ss
	inner join rbd_Governments as g on g.GovernmentID = ss.GovernmentID
	left join rbd_Auditoriums as au on ss.StationID = au.StationID
	left join rbd_Schools as s on s.SchoolID = ss.SchoolFK
	left join rbd_StationProperties as sp on sp.StationID = ss.StationID and sp.PropertyType = 1 --������\������
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