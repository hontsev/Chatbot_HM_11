param($installPath, $toolsPath, $package, $project)

foreach($dir in $project.ProjectItems.Item("Newbe.CQP.Framework").ProjectItems){
	foreach($file in $dir.ProjectItems){
		$configItem = $file
		# set 'Build Action' to 'None'
		$buildAction = $configItem.Properties.Item("BuildAction")
		$buildAction.Value = 0
	}
}

Start-Process "http://www.newbe.cf/"
Start-Process "https://jq.qq.com/?_wv=1027&k=4AMMCTx"
Start-Process "http://git.oschina.net/yks/Newbe.Mahua.Framework"
Start-Process "http://yks.oschina.io/newbe.cqp.framework/"