//Stack Function
//By Conan
//Remove stacked users command and kill stacked players on user death by Dominoes

function serverCmdStackme(%cl)
{
	if(!%cl.isAdmin)
		return;
	%pl = %cl.player;
	if(isObject(%pl))
	{
		%pl.client = "";
		%pl.setName("stackPlayer_" @ %cl.bl_id);
		%cl.player = "";
		%cl.createPlayer(%pl.getTransform());
		%cl.player.mountObject(%pl, 5);
	}
}

function serverCmdClearStack(%cl)
{
	if(!%cl.isAdmin)
		return;
	%pl = %cl.player;
	while(isObject(%mount = %pl.getMountedObject(0)))
		%mount.chainDisappear();
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

package StackMe_ClearChecks
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
activatePackage(StackMe_ClearChecks);
