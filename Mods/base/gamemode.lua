print("Sup")

CE_AddTeam({
	name="The Sussies",
	id="sussies",
	color=CE_CreateColor(0,255,0),
	teamblurb = "The sussiest around",
	layer = 0
})

CE_AddRole({
	name="A much more sensible test role name",
	id="normal",
	team = "sussies",
	color=CE_CreateColor(255,0,255),
	abilities = {Enum_RoleSpecials_Vent, Enum_RoleSpecials_Kill},
	smallblurb = "be NORMAL loser!!!!",
	tasktext = "NORMALITY!!",
	doestasks = true
})

function CheckEndCriteria(isSab,isTaskComplete)

end

function CanKill(attacker, victim)
	return true
end