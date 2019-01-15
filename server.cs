//Stack Function
//By Conan
//Remove stacked users command and kill stacked players on user death by Dominoes

function serverCmdStackme(%cl)
{
	if(!%cl.isModerator || !%cl.isAdmin)
	{
		%this.hasStackLimit = 1;
		%this.stackLimit = 15;
	}
	if(%cl.isModerator)
	{
		%this.hasStackLimit = 1;
		%this.stackLimit = 25;
	}
	if(%cl.isAdmin)
		%this.hasStackLimit = 0;
	
	%pl = %cl.player;
	if(isObject(%pl))
	{
		if(%pl.stackCount > %pl.stackLimit)
			return;
		
		%pl.client = "";
		%pl.setName("stackPlayer_" @ %cl.bl_id);
		%cl.player = "";
		%cl.createPlayer(%pl.getTransform());
		%cl.player.mountObject(%pl, 5);
		if(%cl.hasStackLimit)
			%pl.stackCount++;
	}
}

function serverCmdStackUser(%cl,%this)
{
	%t = findclientbyname(%this);
	
	if(!%cl.isAdmin)
		return;
	
	serverCmdStackMe(%this);
}

function serverCmdClearStack(%cl)
{
	if(!%cl.isAdmin)
		return;
	%pl = %cl.player;
	while(isObject(%mount = %pl.getMountedObject(0)))
		%mount.chainDisappear();
	
	if(%cl.hasStackLimit)
		%pl.stackCount = 0;
}

function Player::chainKill(%pl)
{
	while(isObject(%mount = %pl.getMountedObject(0)))
	{
		%mount.setShapeName("",8564862);
		%mount.chainKill();
	}
	%pl.kill();
}

function Player::chainDisappear(%pl)
{
	while(isObject(%mount = %pl.getMountedObject(0)))
		%mount.chainDisappear();
	
	%pl.delete();
}

package StackMe_Checks
{
	function GameConnection::OnDeath(%client, %sourceObject, %sourceClient, %damageType, %damLoc)
	{
		while(isObject(%targetStack = "stackPlayer_" @ %cl.bl_id))
		{
			%z = getWord(%mount.getScale(),2);
			%targetStack.spawnExplosion(SpawnProjectile,%z);
			%targetStack.setShapeName("",8564862);
			%targetStack.delete();
		}
		parent::onDeath(%client, %sourceObject, %sourceClient, %damageType, %damLoc);
	}
	
	function GameConnection::onClientLeaveGame(%cl)
	{
		while(isObject(%targetStack = "stackPlayer_" @ %cl.bl_id))
		{
			%z = getWord(%mount.getScale(),2);
			%targetStack.spawnExplosion(SpawnProjectile,%z);
			%targetStack.setShapeName("",8564862);
			%targetStack.delete();
		}
		parent::onClientLeaveGame(%cl);
	}
};
activatePackage(StackMe_Checks);
