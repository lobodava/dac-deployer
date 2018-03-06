using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.SqlServer.Dac;
using static System.String;

namespace DacDeployer.Helpers
{
	public static class DacDeployOptionsCreator
	{

		public static DacDeployOptions CreateDacDeployOptions (XmlDocument doc, XmlNamespaceManager ns) 
		{
			var dacDeployOptions =  new DacDeployOptions();
			
			AddBooleanOptions(doc, ns, dacDeployOptions);

			AddNonBooleanOptions(doc, ns, dacDeployOptions);

			AddDoNotDropOptions(doc, ns, dacDeployOptions);

			AddExcludeOptions(doc, ns, dacDeployOptions);

			AddSqlCmdVariables(doc, ns, dacDeployOptions);

			return dacDeployOptions;
		}


		private static void AddBooleanOptions(XmlDocument doc, XmlNamespaceManager ns, DacDeployOptions dacDeployOptions)
		{
			var properties = dacDeployOptions.GetType().GetProperties();

			foreach (var property in properties)
			{
				if (property.PropertyType == typeof(bool) && property.CanWrite)
				{
					var optionValue = PublishProfileXmlHelper.GetBooleanOptionValue(doc, ns, property.Name, null);

					if (optionValue != null)
						property.SetValue(dacDeployOptions, optionValue.Value);
				}
			}
		}

		private static void AddNonBooleanOptions(XmlDocument doc, XmlNamespaceManager ns, DacDeployOptions dacDeployOptions)
		{
			if (int.TryParse(PublishProfileXmlHelper.GetOptionalXmlNodeValue(doc, ns, "//ns:Project/ns:PropertyGroup/ns:CommandTimeout"),
				out int commandTimeout))
				dacDeployOptions.CommandTimeout = commandTimeout;

			var stringOptionValue =
				PublishProfileXmlHelper.GetOptionalXmlNodeValue(doc, ns, "//ns:Project/ns:PropertyGroup/ns:AdditionalDeploymentContributors");

			if (!IsNullOrWhiteSpace(stringOptionValue))
				dacDeployOptions.AdditionalDeploymentContributors = stringOptionValue;

			stringOptionValue =
				PublishProfileXmlHelper.GetOptionalXmlNodeValue(doc, ns, "//ns:Project/ns:PropertyGroup/ns:AdditionalDeploymentContributorArguments");

			if (!IsNullOrWhiteSpace(stringOptionValue))
				dacDeployOptions.AdditionalDeploymentContributorArguments = stringOptionValue;
		}

		private static void AddDoNotDropOptions(XmlDocument doc, XmlNamespaceManager ns, DacDeployOptions dacDeployOptions)
		{
			var objectTypeList = new List<ObjectType>();

			foreach (ObjectType objectType in Enum.GetValues(typeof(ObjectType)))
			{
				var objectTypeName = Enum.GetName(typeof(ObjectType), objectType);

				if (PublishProfileXmlHelper.GetBooleanOptionValue(doc, ns, $"DoNotDrop{objectTypeName}") == true)
					objectTypeList.Add(objectType);
			}

			dacDeployOptions.DoNotDropObjectTypes = objectTypeList.ToArray();
		}

		private static void AddExcludeOptions(XmlDocument doc, XmlNamespaceManager ns, DacDeployOptions dacDeployOptions)
		{
			var objectTypeList = new List<ObjectType>();

			foreach (ObjectType objectType in Enum.GetValues(typeof(ObjectType)))
			{
				var objectTypeName = Enum.GetName(typeof(ObjectType), objectType);

				if (PublishProfileXmlHelper.GetBooleanOptionValue(doc, ns, $"Exclude{objectTypeName}") == true)
					objectTypeList.Add(objectType);
			}

			dacDeployOptions.ExcludeObjectTypes = objectTypeList.ToArray();
		}
		
		private static void AddSqlCmdVariables(XmlDocument doc, XmlNamespaceManager ns, DacDeployOptions dacDeployOptions)
		{
			var varNodeList = doc.SelectNodes("//ns:Project/ns:ItemGroup/ns:SqlCmdVariable", ns);

			if (varNodeList == null) return;

			foreach (XmlNode varNode in varNodeList)
			{
				string varName = null, varValue = null;

				if (varNode.Attributes != null)
					varName = varNode.Attributes["Include"].Value;

				if (IsNullOrWhiteSpace(varName))
					continue;

				foreach (XmlNode child in varNode.ChildNodes)
				{
					if (child.Name == "Value")
					{
						varValue = child.InnerText;
						break;
					}
				}

				//if (new[] { "BeforeDeploymentRelativeDirectory", "BeforeDeploymentScriptName", "SqlCmdVariablesScriptName" }.Contains(varName))
				//	continue;

				dacDeployOptions.SqlCommandVariableValues.Add(varName, varValue);
			}
		}

	}
}

//var dacDeployOptions =  new DacDeployOptions {
			//	IncludeCompositeObjects                    = GetOption(doc, ns, "IncludeCompositeObjects"				   ),
			//	BlockOnPossibleDataLoss                    = GetOption(doc, ns, "BlockOnPossibleDataLoss"				   ),
			//	DropObjectsNotInSource                     = GetOption(doc, ns, "DropObjectsNotInSource"				   ),
			//	GenerateSmartDefaults                      = GetOption(doc, ns, "GenerateSmartDefaults"					   ),
			//	CreateNewDatabase                          = GetOption(doc, ns, "CreateNewDatabase"						   ),
			//	DacDeployerInSingleUserMode             = GetOption(doc, ns, "DacDeployerInSingleUserMode"		   ),
			//	BackupDatabaseBeforeChanges                = GetOption(doc, ns, "BackupDatabaseBeforeChanges"			   ),
			//	IgnoreAuthorizer                           = GetOption(doc, ns, "IgnoreAuthorizer"						   ),
			//	IgnoreColumnCollation                      = GetOption(doc, ns, "IgnoreColumnCollation"					   ),
			//	IgnoreColumnOrder                          = GetOption(doc, ns, "IgnoreColumnOrder"						   ),
			//	IgnoreComments                             = GetOption(doc, ns, "IgnoreComments"						   ),
			//	IgnoreDdlTriggerOrder                      = GetOption(doc, ns, "IgnoreDdlTriggerOrder"					   ),
			//	IgnoreDdlTriggerState                      = GetOption(doc, ns, "IgnoreDdlTriggerState"					   ),
			//	IgnoreDefaultSchema                        = GetOption(doc, ns, "IgnoreDefaultSchema"					   ),
			//	IgnoreDmlTriggerOrder                      = GetOption(doc, ns, "IgnoreDmlTriggerOrder"					   ),
			//	AllowDropBlockingAssemblies                = GetOption(doc, ns, "AllowDropBlockingAssemblies"			   ),
			//	AllowIncompatiblePlatform                  = GetOption(doc, ns, "AllowIncompatiblePlatform"				   ),
			//	AllowUnsafeRowLevelSecurityDataMovement    = GetOption(doc, ns, "AllowUnsafeRowLevelSecurityDataMovement"  ),
			//	CommentOutSetVarDeclarations               = GetOption(doc, ns, "CommentOutSetVarDeclarations"			   ),
			//	CompareUsingTargetCollation                = GetOption(doc, ns, "CompareUsingTargetCollation"			   ),
			//	IncludeTransactionalScripts                = GetOption(doc, ns, "IncludeTransactionalScripts"			   ),
			//	ScriptDatabaseCollation                    = GetOption(doc, ns, "ScriptDatabaseCollation"				   ),
			//	ScriptDatabaseCompatibility                = GetOption(doc, ns, "ScriptDatabaseCompatibility"			   ),
			//	ScriptFileSize                             = GetOption(doc, ns, "ScriptFileSize"						   ),
			//	ScriptDeployStateChecks                    = GetOption(doc, ns, "ScriptDeployStateChecks"				   ),
			//	TreatVerificationErrorsAsWarnings          = GetOption(doc, ns, "TreatVerificationErrorsAsWarnings"		   ),
			//	IgnoreDmlTriggerState                      = GetOption(doc, ns, "IgnoreDmlTriggerState"					   ),
			//	IgnoreIdentitySeed                         = GetOption(doc, ns, "IgnoreIdentitySeed"					   ),
			//	IgnoreIncrement                            = GetOption(doc, ns, "IgnoreIncrement"						   ),
			//	IgnoreIndexOptions                         = GetOption(doc, ns, "IgnoreIndexOptions"					   ),
			//	IgnoreLockHintsOnIndexes                   = GetOption(doc, ns, "IgnoreLockHintsOnIndexes"				   ),
			//	IgnoreNotForReplication                    = GetOption(doc, ns, "IgnoreNotForReplication"				   ),
			//	IgnorePartitionSchemes                     = GetOption(doc, ns, "IgnorePartitionSchemes"				   ),
			//	IgnoreTableOptions                         = GetOption(doc, ns, "IgnoreTableOptions"					   ),
			//	IgnoreUserSettingsObjects                  = GetOption(doc, ns, "IgnoreUserSettingsObjects"				   ),
			//	IgnoreWithNocheckOnCheckConstraints        = GetOption(doc, ns, "IgnoreWithNocheckOnCheckConstraints"	   ),
			//	IgnoreWithNocheckOnForeignKeys             = GetOption(doc, ns, "IgnoreWithNocheckOnForeignKeys"		   ),
				
			//	IgnoreExtendedProperties                   = GetOption(doc, ns, "IgnoreExtendedProperties"				   ),
				
			//	IgnorePermissions                          = GetOption(doc, ns, "IgnorePermissions"						   ),
				
			//	IgnoreRoleMembership                       = GetOption(doc, ns, "IgnoreRoleMembership"					   ),
				
			//	DropRoleMembersNotInSource                 = GetOption(doc, ns, "DropRoleMembersNotInSource"			   ),
			//	DropPermissionsNotInSource                 = GetOption(doc, ns, "DropPermissionsNotInSource" 			   ),
			//};



			//if (GetOption(doc, ns, "DoNotDropAggregates"					))	objectTypeList.Add(ObjectType.Aggregates					);
			//if (GetOption(doc, ns, "DoNotDropApplicationRoles"				))	objectTypeList.Add(ObjectType.ApplicationRoles				);
			//if (GetOption(doc, ns, "DoNotDropAssemblies"					))	objectTypeList.Add(ObjectType.Assemblies					);
			//if (GetOption(doc, ns, "DoNotDropAsymmetricKeys"				))	objectTypeList.Add(ObjectType.AsymmetricKeys				);
			//if (GetOption(doc, ns, "DoNotDropAudits"						))	objectTypeList.Add(ObjectType.Audits						);
			//if (GetOption(doc, ns, "DoNotDropBrokerPriorities"				))	objectTypeList.Add(ObjectType.BrokerPriorities				);
			//if (GetOption(doc, ns, "DoNotDropCertificates"					))	objectTypeList.Add(ObjectType.Certificates					);
			//if (GetOption(doc, ns, "DoNotDropClrUserDefinedTypes"			))	objectTypeList.Add(ObjectType.ClrUserDefinedTypes			);
			//if (GetOption(doc, ns, "DoNotDropColumnEncryptionKeys"			))	objectTypeList.Add(ObjectType.ColumnEncryptionKeys			);
			//if (GetOption(doc, ns, "DoNotDropColumnMasterKeys"				))	objectTypeList.Add(ObjectType.ColumnMasterKeys				);
			//if (GetOption(doc, ns, "DoNotDropContracts"					   	))	objectTypeList.Add(ObjectType.Contracts						);
			//if (GetOption(doc, ns, "DoNotDropCredentials"					))	objectTypeList.Add(ObjectType.Credentials					);
			//if (GetOption(doc, ns, "DoNotDropCryptographicProviders"		))	objectTypeList.Add(ObjectType.CryptographicProviders		);
			//if (GetOption(doc, ns, "DoNotDropDatabaseAuditSpecifications"	))	objectTypeList.Add(ObjectType.DatabaseAuditSpecifications	);
			//if (GetOption(doc, ns, "DoNotDropDatabaseRoles"				   	))	objectTypeList.Add(ObjectType.DatabaseRoles					);
			//if (GetOption(doc, ns, "DoNotDropDatabaseScopedCredentials"	   	))	objectTypeList.Add(ObjectType.DatabaseScopedCredentials		);
			//if (GetOption(doc, ns, "DoNotDropDatabaseTriggers"				))	objectTypeList.Add(ObjectType.DatabaseTriggers				);
			//if (GetOption(doc, ns, "DoNotDropDefaults"						))	objectTypeList.Add(ObjectType.Defaults						);
			//if (GetOption(doc, ns, "DoNotDropErrorMessages"				   	))	objectTypeList.Add(ObjectType.ErrorMessages					);
			//if (GetOption(doc, ns, "DoNotDropEndpoints"					   	))	objectTypeList.Add(ObjectType.Endpoints						);
			//if (GetOption(doc, ns, "DoNotDropEventNotifications"			))	objectTypeList.Add(ObjectType.EventNotifications			);
			//if (GetOption(doc, ns, "DoNotDropEventSessions"				   	))	objectTypeList.Add(ObjectType.EventSessions					);
			//if (GetOption(doc, ns, "DoNotDropExtendedProperties"			))	objectTypeList.Add(ObjectType.ExtendedProperties			);
			//if (GetOption(doc, ns, "DoNotDropExternalDataSources"			))	objectTypeList.Add(ObjectType.ExternalDataSources			);
			//if (GetOption(doc, ns, "DoNotDropExternalFileFormats"			))	objectTypeList.Add(ObjectType.ExternalFileFormats			);
			//if (GetOption(doc, ns, "DoNotDropExternalTables"				))	objectTypeList.Add(ObjectType.ExternalTables				);
			//if (GetOption(doc, ns, "DoNotDropFileTables"					))	objectTypeList.Add(ObjectType.FileTables					);
			//if (GetOption(doc, ns, "DoNotDropFilegroups"					))	objectTypeList.Add(ObjectType.Filegroups					);
			//if (GetOption(doc, ns, "DoNotDropFullTextCatalogs"				))	objectTypeList.Add(ObjectType.FullTextCatalogs				);
			//if (GetOption(doc, ns, "DoNotDropFullTextStoplists"			   	))	objectTypeList.Add(ObjectType.FullTextStoplists				);
			//if (GetOption(doc, ns, "DoNotDropLinkedServerLogins"			))	objectTypeList.Add(ObjectType.LinkedServerLogins			);
			//if (GetOption(doc, ns, "DoNotDropLinkedServers"				   	))	objectTypeList.Add(ObjectType.LinkedServers					);
			//if (GetOption(doc, ns, "DoNotDropLogins"						))	objectTypeList.Add(ObjectType.Logins						);
			//if (GetOption(doc, ns, "DoNotDropMessageTypes"					))	objectTypeList.Add(ObjectType.MessageTypes					);
			//if (GetOption(doc, ns, "DoNotDropPartitionFunctions"			))	objectTypeList.Add(ObjectType.PartitionFunctions			);
			//if (GetOption(doc, ns, "DoNotDropPartitionSchemes"				))	objectTypeList.Add(ObjectType.PartitionSchemes				);
			//if (GetOption(doc, ns, "DoNotDropPermissions"					))	objectTypeList.Add(ObjectType.Permissions					);
			//if (GetOption(doc, ns, "DoNotDropQueues"						))	objectTypeList.Add(ObjectType.Queues						);
			//if (GetOption(doc, ns, "DoNotDropRemoteServiceBindings"		   	))	objectTypeList.Add(ObjectType.RemoteServiceBindings			);
			//if (GetOption(doc, ns, "DoNotDropRoleMembership"				))	objectTypeList.Add(ObjectType.RoleMembership				);
			//if (GetOption(doc, ns, "DoNotDropRoutes"						))	objectTypeList.Add(ObjectType.Routes						);
			//if (GetOption(doc, ns, "DoNotDropRules"						   	))	objectTypeList.Add(ObjectType.Rules							);
			//if (GetOption(doc, ns, "DoNotDropScalarValuedFunctions"		   	))	objectTypeList.Add(ObjectType.ScalarValuedFunctions			);
			//if (GetOption(doc, ns, "DoNotDropSearchPropertyLists"			))	objectTypeList.Add(ObjectType.SearchPropertyLists			);
			//if (GetOption(doc, ns, "DoNotDropSecurityPolicies"				))	objectTypeList.Add(ObjectType.SecurityPolicies				);
			//if (GetOption(doc, ns, "DoNotDropSequences"					   	))	objectTypeList.Add(ObjectType.Sequences						);
			//if (GetOption(doc, ns, "DoNotDropServerAuditSpecifications"	   	))	objectTypeList.Add(ObjectType.ServerAuditSpecifications		);
			//if (GetOption(doc, ns, "DoNotDropServerRoleMembership"			))	objectTypeList.Add(ObjectType.ServerRoleMembership			);
			//if (GetOption(doc, ns, "DoNotDropServerRoles"					))	objectTypeList.Add(ObjectType.ServerRoles					);
			//if (GetOption(doc, ns, "DoNotDropServerTriggers"				))	objectTypeList.Add(ObjectType.ServerTriggers				);
			//if (GetOption(doc, ns, "DoNotDropServices"						))	objectTypeList.Add(ObjectType.Services						);
			//if (GetOption(doc, ns, "DoNotDropSignatures"					))	objectTypeList.Add(ObjectType.Signatures					);
			//if (GetOption(doc, ns, "DoNotDropStoredProcedures"				))	objectTypeList.Add(ObjectType.StoredProcedures				);
			//if (GetOption(doc, ns, "DoNotDropSymmetricKeys"				   	))	objectTypeList.Add(ObjectType.SymmetricKeys					);
			//if (GetOption(doc, ns, "DoNotDropSynonyms"						))	objectTypeList.Add(ObjectType.Synonyms						);
			//if (GetOption(doc, ns, "DoNotDropTableValuedFunctions"			))	objectTypeList.Add(ObjectType.TableValuedFunctions			);
			//if (GetOption(doc, ns, "DoNotDropTables"						))	objectTypeList.Add(ObjectType.Tables						);
			//if (GetOption(doc, ns, "DoNotDropUserDefinedDataTypes"			))	objectTypeList.Add(ObjectType.UserDefinedDataTypes			);
			//if (GetOption(doc, ns, "DoNotDropUserDefinedTableTypes"		   	))	objectTypeList.Add(ObjectType.UserDefinedTableTypes			);
			//if (GetOption(doc, ns, "DoNotDropUsers"						   	))	objectTypeList.Add(ObjectType.Users							);
			//if (GetOption(doc, ns, "DoNotDropViews"						   	))	objectTypeList.Add(ObjectType.Views							);
			//if (GetOption(doc, ns, "DoNotDropXmlSchemaCollections"			))	objectTypeList.Add(ObjectType.XmlSchemaCollections			);

			//if (GetOption(doc, ns, "ExcludeColumnEncryptionKeys"			))	objectTypeList.Add(ObjectType.ColumnEncryptionKeys          );
			//if (GetOption(doc, ns, "ExcludeColumnMasterKeys"				))	objectTypeList.Add(ObjectType.ColumnMasterKeys              );
			//if (GetOption(doc, ns, "ExcludeContracts"						))	objectTypeList.Add(ObjectType.Contracts                     );
			//if (GetOption(doc, ns, "ExcludeCredentials"						))	objectTypeList.Add(ObjectType.Credentials                   );
			//if (GetOption(doc, ns, "ExcludeCryptographicProviders"			))	objectTypeList.Add(ObjectType.CryptographicProviders        );
			//if (GetOption(doc, ns, "ExcludeDatabaseAuditSpecifications"		))	objectTypeList.Add(ObjectType.DatabaseAuditSpecifications   );
			//if (GetOption(doc, ns, "ExcludeDatabaseRoles"					))	objectTypeList.Add(ObjectType.DatabaseRoles                 );
			//if (GetOption(doc, ns, "ExcludeDatabaseScopedCredentials"		))	objectTypeList.Add(ObjectType.DatabaseScopedCredentials     );
			//if (GetOption(doc, ns, "ExcludeDatabaseTriggers"				))	objectTypeList.Add(ObjectType.DatabaseTriggers              );
			//if (GetOption(doc, ns, "ExcludeDefaults"						))	objectTypeList.Add(ObjectType.Defaults                      );
			//if (GetOption(doc, ns, "ExcludeEndpoints"						))	objectTypeList.Add(ObjectType.Endpoints                     );
			//if (GetOption(doc, ns, "ExcludeErrorMessages"					))	objectTypeList.Add(ObjectType.ErrorMessages                 );
			//if (GetOption(doc, ns, "ExcludeEventNotifications"				))	objectTypeList.Add(ObjectType.EventNotifications            );
			//if (GetOption(doc, ns, "ExcludeEventSessions"					))	objectTypeList.Add(ObjectType.EventSessions                 );
			//if (GetOption(doc, ns, "ExcludeExternalDataSources"				))	objectTypeList.Add(ObjectType.ExternalDataSources           );
			//if (GetOption(doc, ns, "ExcludeExternalFileFormats"				))	objectTypeList.Add(ObjectType.ExternalFileFormats           );
			//if (GetOption(doc, ns, "ExcludeExternalTables"					))	objectTypeList.Add(ObjectType.ExternalTables                );
			//if (GetOption(doc, ns, "ExcludeFileTables"						))	objectTypeList.Add(ObjectType.FileTables                    );
			//if (GetOption(doc, ns, "ExcludeFilegroups"						))	objectTypeList.Add(ObjectType.Filegroups                    );
			//if (GetOption(doc, ns, "ExcludeFullTextCatalogs"				))	objectTypeList.Add(ObjectType.FullTextCatalogs              );
			//if (GetOption(doc, ns, "ExcludeFullTextStoplists"				))	objectTypeList.Add(ObjectType.FullTextStoplists             );
			//if (GetOption(doc, ns, "ExcludeLinkedServerLogins"				))	objectTypeList.Add(ObjectType.LinkedServerLogins            );
			//if (GetOption(doc, ns, "ExcludeLinkedServers"					))	objectTypeList.Add(ObjectType.LinkedServers                 );
			//if (GetOption(doc, ns, "ExcludeLogins"							))	objectTypeList.Add(ObjectType.Logins                        );
			//if (GetOption(doc, ns, "ExcludeMessageTypes"					))	objectTypeList.Add(ObjectType.MessageTypes                  );
			//if (GetOption(doc, ns, "ExcludePartitionFunctions"				))	objectTypeList.Add(ObjectType.PartitionFunctions            );
			//if (GetOption(doc, ns, "ExcludePartitionSchemes"				))	objectTypeList.Add(ObjectType.PartitionSchemes              );
			//if (GetOption(doc, ns, "ExcludeQueues"							))	objectTypeList.Add(ObjectType.Queues                        );
			//if (GetOption(doc, ns, "ExcludeRemoteServiceBindings"			))	objectTypeList.Add(ObjectType.RemoteServiceBindings         );
			//if (GetOption(doc, ns, "ExcludeRoutes"							))	objectTypeList.Add(ObjectType.Routes                        );
			//if (GetOption(doc, ns, "ExcludeRules"							))	objectTypeList.Add(ObjectType.Rules                         );
			//if (GetOption(doc, ns, "ExcludeScalarValuedFunctions"			))	objectTypeList.Add(ObjectType.ScalarValuedFunctions         );
			//if (GetOption(doc, ns, "ExcludeSearchPropertyLists"				))	objectTypeList.Add(ObjectType.SearchPropertyLists           );
			//if (GetOption(doc, ns, "ExcludeSecurityPolicies"				))	objectTypeList.Add(ObjectType.SecurityPolicies              );
			//if (GetOption(doc, ns, "ExcludeSequences"						))	objectTypeList.Add(ObjectType.Sequences                     );
			//if (GetOption(doc, ns, "ExcludeServerAuditSpecifications"		))	objectTypeList.Add(ObjectType.ServerAuditSpecifications     );
			//if (GetOption(doc, ns, "ExcludeServerRoleMembership"			))	objectTypeList.Add(ObjectType.ServerRoleMembership          );
			//if (GetOption(doc, ns, "ExcludeServerRoles"						))	objectTypeList.Add(ObjectType.ServerRoles                   );
			//if (GetOption(doc, ns, "ExcludeServerTriggers"					))	objectTypeList.Add(ObjectType.ServerTriggers                );
			//if (GetOption(doc, ns, "ExcludeServices"						))	objectTypeList.Add(ObjectType.Services                      );
			//if (GetOption(doc, ns, "ExcludeSignatures"						))	objectTypeList.Add(ObjectType.Signatures                    );
			//if (GetOption(doc, ns, "ExcludeStoredProcedures"				))	objectTypeList.Add(ObjectType.StoredProcedures              );
			//if (GetOption(doc, ns, "ExcludeSymmetricKeys"					))	objectTypeList.Add(ObjectType.SymmetricKeys                 );
			//if (GetOption(doc, ns, "ExcludeSynonyms"						))	objectTypeList.Add(ObjectType.Synonyms                      );
			//if (GetOption(doc, ns, "ExcludeTableValuedFunctions"			))	objectTypeList.Add(ObjectType.TableValuedFunctions          );
			//if (GetOption(doc, ns, "ExcludeTables"							))	objectTypeList.Add(ObjectType.Tables                        );
			//if (GetOption(doc, ns, "ExcludeUserDefinedDataTypes"			))	objectTypeList.Add(ObjectType.UserDefinedDataTypes          );
			//if (GetOption(doc, ns, "ExcludeUserDefinedTableTypes"			))	objectTypeList.Add(ObjectType.UserDefinedTableTypes         );
			//if (GetOption(doc, ns, "ExcludeUsers"							))	objectTypeList.Add(ObjectType.Users                         );
			//if (GetOption(doc, ns, "ExcludeViews"							))	objectTypeList.Add(ObjectType.Views                         );
			//if (GetOption(doc, ns, "ExcludeXmlSchemaCollections"			))	objectTypeList.Add(ObjectType.XmlSchemaCollections          );
		



/*
IncludeCompositeObjects                    = GetOption(doc, ns, "IncludeCompositeObjects"				   ),
BlockOnPossibleDataLoss                    = GetOption(doc, ns, "BlockOnPossibleDataLoss"				   ),
DropObjectsNotInSource                     = GetOption(doc, ns, "DropObjectsNotInSource"				   ),
GenerateSmartDefaults                      = GetOption(doc, ns, "GenerateSmartDefaults"					   ),
CreateNewDatabase                          = GetOption(doc, ns, "CreateNewDatabase"						   ),
DacDeployerInSingleUserMode             = GetOption(doc, ns, "DacDeployerInSingleUserMode"		   ),
BackupDatabaseBeforeChanges                = GetOption(doc, ns, "BackupDatabaseBeforeChanges"			   ),
NoAlterStatementsToChangeCLRTypes          = GetOption(doc, ns, "NoAlterStatementsToChangeCLRTypes"		   ),
DoNotDropAggregates                        = GetOption(doc, ns, "DoNotDropAggregates"					   ),
DoNotDropApplicationRoles                  = GetOption(doc, ns, "DoNotDropApplicationRoles"				   ),
DoNotDropAssemblies                        = GetOption(doc, ns, "DoNotDropAssemblies"					   ),
DoNotDropAsymmetricKeys                    = GetOption(doc, ns, "DoNotDropAsymmetricKeys"				   ),
DoNotDropAudits                            = GetOption(doc, ns, "DoNotDropAudits"						   ),
DoNotDropBrokerPriorities                  = GetOption(doc, ns, "DoNotDropBrokerPriorities"				   ),
DoNotDropCertificates                      = GetOption(doc, ns, "DoNotDropCertificates"					   ),
DoNotDropClrUserDefinedTypes               = GetOption(doc, ns, "DoNotDropClrUserDefinedTypes"			   ),
DoNotDropColumnEncryptionKeys              = GetOption(doc, ns, "DoNotDropColumnEncryptionKeys"			   ),
DoNotDropColumnMasterKeys                  = GetOption(doc, ns, "DoNotDropColumnMasterKeys"				   ),
DoNotDropContracts                         = GetOption(doc, ns, "DoNotDropContracts"					   ),
DoNotDropCredentials                       = GetOption(doc, ns, "DoNotDropCredentials"					   ),
DoNotDropCryptographicProviders            = GetOption(doc, ns, "DoNotDropCryptographicProviders"		   ),
DoNotDropDatabaseAuditSpecifications       = GetOption(doc, ns, "DoNotDropDatabaseAuditSpecifications"	   ),
DoNotDropDatabaseRoles                     = GetOption(doc, ns, "DoNotDropDatabaseRoles"				   ),
DoNotDropDatabaseScopedCredentials         = GetOption(doc, ns, "DoNotDropDatabaseScopedCredentials"	   ),
DoNotDropDatabaseTriggers                  = GetOption(doc, ns, "DoNotDropDatabaseTriggers"				   ),
DoNotDropDefaults                          = GetOption(doc, ns, "DoNotDropDefaults"						   ),
DoNotDropErrorMessages                     = GetOption(doc, ns, "DoNotDropErrorMessages"				   ),
DoNotDropEndpoints                         = GetOption(doc, ns, "DoNotDropEndpoints"					   ),
DoNotDropEventNotifications                = GetOption(doc, ns, "DoNotDropEventNotifications"			   ),
DoNotDropEventSessions                     = GetOption(doc, ns, "DoNotDropEventSessions"				   ),
DoNotDropExtendedProperties                = GetOption(doc, ns, "DoNotDropExtendedProperties"			   ),
DoNotDropExternalDataSources               = GetOption(doc, ns, "DoNotDropExternalDataSources"			   ),
DoNotDropExternalFileFormats               = GetOption(doc, ns, "DoNotDropExternalFileFormats"			   ),
DoNotDropExternalTables                    = GetOption(doc, ns, "DoNotDropExternalTables"				   ),
DoNotDropFileTables                        = GetOption(doc, ns, "DoNotDropFileTables"					   ),
DoNotDropFilegroups                        = GetOption(doc, ns, "DoNotDropFilegroups"					   ),
DoNotDropFullTextCatalogs                  = GetOption(doc, ns, "DoNotDropFullTextCatalogs"				   ),
DoNotDropFullTextStoplists                 = GetOption(doc, ns, "DoNotDropFullTextStoplists"			   ),
DoNotDropLinkedServerLogins                = GetOption(doc, ns, "DoNotDropLinkedServerLogins"			   ),
DoNotDropLinkedServers                     = GetOption(doc, ns, "DoNotDropLinkedServers"				   ),
DoNotDropLogins                            = GetOption(doc, ns, "DoNotDropLogins"						   ),
DoNotDropMessageTypes                      = GetOption(doc, ns, "DoNotDropMessageTypes"					   ),
DoNotDropPartitionFunctions                = GetOption(doc, ns, "DoNotDropPartitionFunctions"			   ),
DoNotDropPartitionSchemes                  = GetOption(doc, ns, "DoNotDropPartitionSchemes"				   ),
DoNotDropPermissions                       = GetOption(doc, ns, "DoNotDropPermissions"					   ),
DoNotDropQueues                            = GetOption(doc, ns, "DoNotDropQueues"						   ),
DoNotDropRemoteServiceBindings             = GetOption(doc, ns, "DoNotDropRemoteServiceBindings"		   ),
DoNotDropRoleMembership                    = GetOption(doc, ns, "DoNotDropRoleMembership"				   ),
DoNotDropRoutes                            = GetOption(doc, ns, "DoNotDropRoutes"						   ),
DoNotDropRules                             = GetOption(doc, ns, "DoNotDropRules"						   ),
DoNotDropScalarValuedFunctions             = GetOption(doc, ns, "DoNotDropScalarValuedFunctions"		   ),
DoNotDropSearchPropertyLists               = GetOption(doc, ns, "DoNotDropSearchPropertyLists"			   ),
DoNotDropSecurityPolicies                  = GetOption(doc, ns, "DoNotDropSecurityPolicies"				   ),
DoNotDropSequences                         = GetOption(doc, ns, "DoNotDropSequences"					   ),
DoNotDropServerAuditSpecifications         = GetOption(doc, ns, "DoNotDropServerAuditSpecifications"	   ),
DoNotDropServerRoleMembership              = GetOption(doc, ns, "DoNotDropServerRoleMembership"			   ),
DoNotDropServerRoles                       = GetOption(doc, ns, "DoNotDropServerRoles"					   ),
DoNotDropServerTriggers                    = GetOption(doc, ns, "DoNotDropServerTriggers"				   ),
DoNotDropServices                          = GetOption(doc, ns, "DoNotDropServices"						   ),
DoNotDropSignatures                        = GetOption(doc, ns, "DoNotDropSignatures"					   ),
DoNotDropStoredProcedures                  = GetOption(doc, ns, "DoNotDropStoredProcedures"				   ),
DoNotDropSymmetricKeys                     = GetOption(doc, ns, "DoNotDropSymmetricKeys"				   ),
DoNotDropSynonyms                          = GetOption(doc, ns, "DoNotDropSynonyms"						   ),
DoNotDropTableValuedFunctions              = GetOption(doc, ns, "DoNotDropTableValuedFunctions"			   ),
DoNotDropTables                            = GetOption(doc, ns, "DoNotDropTables"						   ),
DoNotDropUserDefinedDataTypes              = GetOption(doc, ns, "DoNotDropUserDefinedDataTypes"			   ),
DoNotDropUserDefinedTableTypes             = GetOption(doc, ns, "DoNotDropUserDefinedTableTypes"		   ),
DoNotDropUsers                             = GetOption(doc, ns, "DoNotDropUsers"						   ),
DoNotDropViews                             = GetOption(doc, ns, "DoNotDropViews"						   ),
DoNotDropXmlSchemaCollections              = GetOption(doc, ns, "DoNotDropXmlSchemaCollections"			   ),
IgnoreAuthorizer                           = GetOption(doc, ns, "IgnoreAuthorizer"						   ),
IgnoreColumnCollation                      = GetOption(doc, ns, "IgnoreColumnCollation"					   ),
IgnoreColumnOrder                          = GetOption(doc, ns, "IgnoreColumnOrder"						   ),
IgnoreComments                             = GetOption(doc, ns, "IgnoreComments"						   ),
IgnoreDdlTriggerOrder                      = GetOption(doc, ns, "IgnoreDdlTriggerOrder"					   ),
IgnoreDdlTriggerState                      = GetOption(doc, ns, "IgnoreDdlTriggerState"					   ),
IgnoreDefaultSchema                        = GetOption(doc, ns, "IgnoreDefaultSchema"					   ),
IgnoreDmlTriggerOrder                      = GetOption(doc, ns, "IgnoreDmlTriggerOrder"					   ),
ExcludeAggregates                          = GetOption(doc, ns, "ExcludeAggregates"						   ),
ExcludeApplicationRoles                    = GetOption(doc, ns, "ExcludeApplicationRoles"				   ),
ExcludeAssemblies                          = GetOption(doc, ns, "ExcludeAssemblies"						   ),
ExcludeAsymmetricKeys                      = GetOption(doc, ns, "ExcludeAsymmetricKeys"					   ),
ExcludeAudits                              = GetOption(doc, ns, "ExcludeAudits"							   ),
ExcludeBrokerPriorities                    = GetOption(doc, ns, "ExcludeBrokerPriorities"				   ),
ExcludeCertificates                        = GetOption(doc, ns, "ExcludeCertificates"					   ),
ExcludeClrUserDefinedTypes                 = GetOption(doc, ns, "ExcludeClrUserDefinedTypes"			   ),
AllowDropBlockingAssemblies                = GetOption(doc, ns, "AllowDropBlockingAssemblies"			   ),
AllowIncompatiblePlatform                  = GetOption(doc, ns, "AllowIncompatiblePlatform"				   ),
AllowUnsafeRowLevelSecurityDataMovement    = GetOption(doc, ns, "AllowUnsafeRowLevelSecurityDataMovement"  ),
CommentOutSetVarDeclarations               = GetOption(doc, ns, "CommentOutSetVarDeclarations"			   ),
CompareUsingTargetCollation                = GetOption(doc, ns, "CompareUsingTargetCollation"			   ),
IncludeTransactionalScripts                = GetOption(doc, ns, "IncludeTransactionalScripts"			   ),
ScriptDatabaseCollation                    = GetOption(doc, ns, "ScriptDatabaseCollation"				   ),
ScriptDatabaseCompatibility                = GetOption(doc, ns, "ScriptDatabaseCompatibility"			   ),
ScriptFileSize                             = GetOption(doc, ns, "ScriptFileSize"						   ),
ScriptDeployStateChecks                    = GetOption(doc, ns, "ScriptDeployStateChecks"				   ),
TreatVerificationErrorsAsWarnings          = GetOption(doc, ns, "TreatVerificationErrorsAsWarnings"		   ),
IgnoreDmlTriggerState                      = GetOption(doc, ns, "IgnoreDmlTriggerState"					   ),
IgnoreIdentitySeed                         = GetOption(doc, ns, "IgnoreIdentitySeed"					   ),
IgnoreIncrement                            = GetOption(doc, ns, "IgnoreIncrement"						   ),
IgnoreIndexOptions                         = GetOption(doc, ns, "IgnoreIndexOptions"					   ),
IgnoreLockHintsOnIndexes                   = GetOption(doc, ns, "IgnoreLockHintsOnIndexes"				   ),
IgnoreNotForReplication                    = GetOption(doc, ns, "IgnoreNotForReplication"				   ),
IgnorePartitionSchemes                     = GetOption(doc, ns, "IgnorePartitionSchemes"				   ),
IgnoreTableOptions                         = GetOption(doc, ns, "IgnoreTableOptions"					   ),
IgnoreUserSettingsObjects                  = GetOption(doc, ns, "IgnoreUserSettingsObjects"				   ),
IgnoreWithNocheckOnCheckConstraints        = GetOption(doc, ns, "IgnoreWithNocheckOnCheckConstraints"	   ),
IgnoreWithNocheckOnForeignKeys             = GetOption(doc, ns, "IgnoreWithNocheckOnForeignKeys"		   ),
ExcludeColumnEncryptionKeys                = GetOption(doc, ns, "ExcludeColumnEncryptionKeys"			   ),
ExcludeColumnMasterKeys                    = GetOption(doc, ns, "ExcludeColumnMasterKeys"				   ),
ExcludeContracts                           = GetOption(doc, ns, "ExcludeContracts"						   ),
ExcludeCredentials                         = GetOption(doc, ns, "ExcludeCredentials"					   ),
ExcludeCryptographicProviders              = GetOption(doc, ns, "ExcludeCryptographicProviders"			   ),
ExcludeDatabaseAuditSpecifications         = GetOption(doc, ns, "ExcludeDatabaseAuditSpecifications"	   ),
ExcludeDatabaseRoles                       = GetOption(doc, ns, "ExcludeDatabaseRoles"					   ),
ExcludeDatabaseScopedCredentials           = GetOption(doc, ns, "ExcludeDatabaseScopedCredentials"		   ),
ExcludeDatabaseTriggers                    = GetOption(doc, ns, "ExcludeDatabaseTriggers"				   ),
ExcludeDefaults                            = GetOption(doc, ns, "ExcludeDefaults"						   ),
ExcludeEndpoints                           = GetOption(doc, ns, "ExcludeEndpoints"						   ),
ExcludeErrorMessages                       = GetOption(doc, ns, "ExcludeErrorMessages"					   ),
ExcludeEventNotifications                  = GetOption(doc, ns, "ExcludeEventNotifications"				   ),
ExcludeEventSessions                       = GetOption(doc, ns, "ExcludeEventSessions"					   ),
IgnoreExtendedProperties                   = GetOption(doc, ns, "IgnoreExtendedProperties"				   ),
ExcludeExternalDataSources                 = GetOption(doc, ns, "ExcludeExternalDataSources"			   ),
ExcludeExternalFileFormats                 = GetOption(doc, ns, "ExcludeExternalFileFormats"			   ),
ExcludeExternalTables                      = GetOption(doc, ns, "ExcludeExternalTables"					   ),
ExcludeFileTables                          = GetOption(doc, ns, "ExcludeFileTables"						   ),
ExcludeFilegroups                          = GetOption(doc, ns, "ExcludeFilegroups"						   ),
ExcludeFullTextCatalogs                    = GetOption(doc, ns, "ExcludeFullTextCatalogs"				   ),
ExcludeFullTextStoplists                   = GetOption(doc, ns, "ExcludeFullTextStoplists"				   ),
ExcludeLinkedServerLogins                  = GetOption(doc, ns, "ExcludeLinkedServerLogins"				   ),
ExcludeLinkedServers                       = GetOption(doc, ns, "ExcludeLinkedServers"					   ),
ExcludeLogins                              = GetOption(doc, ns, "ExcludeLogins"							   ),
ExcludeMessageTypes                        = GetOption(doc, ns, "ExcludeMessageTypes"					   ),
ExcludePartitionFunctions                  = GetOption(doc, ns, "ExcludePartitionFunctions"				   ),
ExcludePartitionSchemes                    = GetOption(doc, ns, "ExcludePartitionSchemes"				   ),
IgnorePermissions                          = GetOption(doc, ns, "IgnorePermissions"						   ),
ExcludeQueues                              = GetOption(doc, ns, "ExcludeQueues"							   ),
ExcludeRemoteServiceBindings               = GetOption(doc, ns, "ExcludeRemoteServiceBindings"			   ),
IgnoreRoleMembership                       = GetOption(doc, ns, "IgnoreRoleMembership"					   ),
ExcludeRoutes                              = GetOption(doc, ns, "ExcludeRoutes"							   ),
ExcludeRules                               = GetOption(doc, ns, "ExcludeRules"							   ),
ExcludeScalarValuedFunctions               = GetOption(doc, ns, "ExcludeScalarValuedFunctions"			   ),
ExcludeSearchPropertyLists                 = GetOption(doc, ns, "ExcludeSearchPropertyLists"			   ),
ExcludeSecurityPolicies                    = GetOption(doc, ns, "ExcludeSecurityPolicies"				   ),
ExcludeSequences                           = GetOption(doc, ns, "ExcludeSequences"						   ),
ExcludeServerAuditSpecifications           = GetOption(doc, ns, "ExcludeServerAuditSpecifications"		   ),
ExcludeServerRoleMembership                = GetOption(doc, ns, "ExcludeServerRoleMembership"			   ),
ExcludeServerRoles                         = GetOption(doc, ns, "ExcludeServerRoles"					   ),
ExcludeServerTriggers                      = GetOption(doc, ns, "ExcludeServerTriggers"					   ),
ExcludeServices                            = GetOption(doc, ns, "ExcludeServices"						   ),
ExcludeSignatures                          = GetOption(doc, ns, "ExcludeSignatures"						   ),
ExcludeStoredProcedures                    = GetOption(doc, ns, "ExcludeStoredProcedures"				   ),
ExcludeSymmetricKeys                       = GetOption(doc, ns, "ExcludeSymmetricKeys"					   ),
ExcludeSynonyms                            = GetOption(doc, ns, "ExcludeSynonyms"						   ),
ExcludeTableValuedFunctions                = GetOption(doc, ns, "ExcludeTableValuedFunctions"			   ),
ExcludeTables                              = GetOption(doc, ns, "ExcludeTables"							   ),
ExcludeUserDefinedDataTypes                = GetOption(doc, ns, "ExcludeUserDefinedDataTypes"			   ),
ExcludeUserDefinedTableTypes               = GetOption(doc, ns, "ExcludeUserDefinedTableTypes"			   ),
ExcludeUsers                               = GetOption(doc, ns, "ExcludeUsers"							   ),
ExcludeViews                               = GetOption(doc, ns, "ExcludeViews"							   ),
ExcludeXmlSchemaCollections                = GetOption(doc, ns, "ExcludeXmlSchemaCollections"			   ),
DropRoleMembersNotInSource                 = GetOption(doc, ns, "DropRoleMembersNotInSource"			   ),
DropPermissionsNotInSource                 = GetOption(doc, ns, "DropPermissionsNotInSource" 			   ),

IncludeCompositeObjects
BlockOnPossibleDataLoss
DropObjectsNotInSource
GenerateSmartDefaults
CreateNewDatabase
DacDeployerInSingleUserMode
BackupDatabaseBeforeChanges
NoAlterStatementsToChangeCLRTypes
DoNotDropAggregates
DoNotDropApplicationRoles
DoNotDropAssemblies
DoNotDropAsymmetricKeys
DoNotDropAudits
DoNotDropBrokerPriorities
DoNotDropCertificates
DoNotDropClrUserDefinedTypes
DoNotDropColumnEncryptionKeys
DoNotDropColumnMasterKeys
DoNotDropContracts
DoNotDropCredentials
DoNotDropCryptographicProviders
DoNotDropDatabaseAuditSpecifications
DoNotDropDatabaseRoles
DoNotDropDatabaseScopedCredentials
DoNotDropDatabaseTriggers
DoNotDropDefaults
DoNotDropErrorMessages
DoNotDropEndpoints
DoNotDropEventNotifications
DoNotDropEventSessions
DoNotDropExtendedProperties
DoNotDropExternalDataSources
DoNotDropExternalFileFormats
DoNotDropExternalTables
DoNotDropFileTables
DoNotDropFilegroups
DoNotDropFullTextCatalogs
DoNotDropFullTextStoplists
DoNotDropLinkedServerLogins
DoNotDropLinkedServers
DoNotDropLogins
DoNotDropMessageTypes
DoNotDropPartitionFunctions
DoNotDropPartitionSchemes
DoNotDropPermissions
DoNotDropQueues
DoNotDropRemoteServiceBindings
DoNotDropRoleMembership
DoNotDropRoutes
DoNotDropRules
DoNotDropScalarValuedFunctions
DoNotDropSearchPropertyLists
DoNotDropSecurityPolicies
DoNotDropSequences
DoNotDropServerAuditSpecifications
DoNotDropServerRoleMembership
DoNotDropServerRoles
DoNotDropServerTriggers
DoNotDropServices
DoNotDropSignatures
DoNotDropStoredProcedures
DoNotDropSymmetricKeys
DoNotDropSynonyms
DoNotDropTableValuedFunctions
DoNotDropTables
DoNotDropUserDefinedDataTypes
DoNotDropUserDefinedTableTypes
DoNotDropUsers
DoNotDropViews
DoNotDropXmlSchemaCollections
IgnoreAuthorizer
IgnoreColumnCollation
IgnoreColumnOrder
IgnoreComments
IgnoreDdlTriggerOrder
IgnoreDdlTriggerState
IgnoreDefaultSchema
IgnoreDmlTriggerOrder
ExcludeAggregates
ExcludeApplicationRoles
ExcludeAssemblies
ExcludeAsymmetricKeys
ExcludeAudits
ExcludeBrokerPriorities
ExcludeCertificates
ExcludeClrUserDefinedTypes
AllowDropBlockingAssemblies
AllowIncompatiblePlatform
AllowUnsafeRowLevelSecurityDataMovement
CommentOutSetVarDeclarations
CompareUsingTargetCollation
IncludeTransactionalScripts
ScriptDatabaseCollation
ScriptDatabaseCompatibility
ScriptFileSize
ScriptDeployStateChecks
TreatVerificationErrorsAsWarnings
IgnoreDmlTriggerState
IgnoreIdentitySeed
IgnoreIncrement
IgnoreIndexOptions
IgnoreLockHintsOnIndexes
IgnoreNotForReplication
IgnorePartitionSchemes
IgnoreTableOptions
IgnoreUserSettingsObjects
IgnoreWithNocheckOnCheckConstraints
IgnoreWithNocheckOnForeignKeys
ExcludeColumnEncryptionKeys
ExcludeColumnMasterKeys
ExcludeContracts
ExcludeCredentials
ExcludeCryptographicProviders
ExcludeDatabaseAuditSpecifications
ExcludeDatabaseRoles
ExcludeDatabaseScopedCredentials
ExcludeDatabaseTriggers
ExcludeDefaults
ExcludeEndpoints
ExcludeErrorMessages
ExcludeEventNotifications
ExcludeEventSessions
IgnoreExtendedProperties
ExcludeExternalDataSources
ExcludeExternalFileFormats
ExcludeExternalTables
ExcludeFileTables
ExcludeFilegroups
ExcludeFullTextCatalogs
ExcludeFullTextStoplists
ExcludeLinkedServerLogins
ExcludeLinkedServers
ExcludeLogins
ExcludeMessageTypes
ExcludePartitionFunctions
ExcludePartitionSchemes
IgnorePermissions
ExcludeQueues
ExcludeRemoteServiceBindings
IgnoreRoleMembership
ExcludeRoutes
ExcludeRules
ExcludeScalarValuedFunctions
ExcludeSearchPropertyLists
ExcludeSecurityPolicies
ExcludeSequences
ExcludeServerAuditSpecifications
ExcludeServerRoleMembership
ExcludeServerRoles
ExcludeServerTriggers
ExcludeServices
ExcludeSignatures
ExcludeStoredProcedures
ExcludeSymmetricKeys
ExcludeSynonyms
ExcludeTableValuedFunctions
ExcludeTables
ExcludeUserDefinedDataTypes
ExcludeUserDefinedTableTypes
ExcludeUsers
ExcludeViews
ExcludeXmlSchemaCollections
DropRoleMembersNotInSource
DropPermissionsNotInSource


*/								