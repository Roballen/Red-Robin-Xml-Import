[T4Scaffolding.Scaffolder(Description = "Scaffold your PetaPoco controllers and views")][CmdletBinding()]
param(        
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$ControllerName,
    [string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false,
	[switch]$ViewsOnly = $false
)

$outputPath = "Controllers\$ControllerName"+"Controller" 

#NEED TO GET SOME TYPE INFORMATION FOR THE VIEWS..

$DataView =  Get-ProjectType $ControllerName"Poco" -Project $Project

#SET THE PATHS FOR THE VIEWS.
$detailsViewOutPath = "Views\$ControllerName\Details"
$createViewOutPath = "Views\$ControllerName\Create"
$editViewOutPath = "Views\$ControllerName\Edit"
$deleteViewOutPath = "Views\$ControllerName\Delete"
$indexViewOutPath = "Views\$ControllerName\Index"

# SET THE NAMESPACE
$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value

if(!$ViewsOnly){
Add-ProjectItemViaTemplate $outputPath -Template PetaPocoControllerTemplate `
	-Model @{ Namespace = $namespace; ControllerName = $ControllerName } `
	-SuccessMessage "Added PetaPocoScaffolding output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force
}

############ NOW WE HAVE CREATED THE CONTROLLER WE CAN CREATE THE VIEWS TOO. ############

#INDEX
Add-ProjectItemViaTemplate $indexViewOutPath -Template PetaPocoViewIndexTemplate `
	-Model @{ Namespace = $namespace; ControllerName = $ControllerName; DataView = [MarshalByRefObject]$DataView } `
	-SuccessMessage "Added PetaPocoScaffolding output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

#DETAILS
Add-ProjectItemViaTemplate $detailsViewOutPath -Template PetaPocoViewDetailTemplate `
	-Model @{ Namespace = $namespace; ControllerName = $ControllerName; DataView = [MarshalByRefObject]$DataView } `
	-SuccessMessage "Added PetaPocoScaffolding output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

#CREATE
Add-ProjectItemViaTemplate $createViewOutPath -Template PetaPocoViewCreateTemplate `
	-Model @{ Namespace = $namespace; ControllerName = $ControllerName; DataView = [MarshalByRefObject]$DataView } `
	-SuccessMessage "Added PetaPocoScaffolding output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

#EDIT
Add-ProjectItemViaTemplate $editViewOutPath -Template PetaPocoViewEditTemplate `
	-Model @{ Namespace = $namespace; ControllerName = $ControllerName; DataView = [MarshalByRefObject]$DataView } `
	-SuccessMessage "Added PetaPocoScaffolding output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

#DELETE
Add-ProjectItemViaTemplate $deleteViewOutPath -Template PetaPocoViewDeleteTemplate `
	-Model @{ Namespace = $namespace; ControllerName = $ControllerName; DataView = [MarshalByRefObject]$DataView } `
	-SuccessMessage "Added PetaPocoScaffolding output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force