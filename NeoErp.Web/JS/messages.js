var Messages = {};
$(function () {
    Messages = {
        content: {
            save: {
                success: {
                    content: 'Selected Information was successfully saved .',
                    title: 'Selected Information Saved',
                    method: 'success'
                },
                failed: {
                    content: 'Given value is Not Correct Please try again later.',
                    title: 'Failed saving selected content',
                    method: 'error'
                },
                forbidden: {
                    content: 'You are not allowed to change the selected content in the Migration Plan.<br/>Ask an account manager for more privileges.',
                    title: 'You are not allowed to do that',
                    method: 'warning'
                },
                noaccess: {
                    content: 'You don\'t have access to this Migration Plan.',
                    title: 'You don\'t have access.',
                    method: 'error'
                },
                validaionfailder: {
                    content: 'Please Check Given Information',
                    title: 'Failed validation.',
                    method: 'error'
                }
            }
        },
        Offer: {
            save: {
                success: {
                    content: 'Candidate is Selected For Offer .',
                    title: 'Offer Selected',
                    method: 'success'
                },
                failed: {
                    content: 'Given value is Not Correct Please try again later.',
                    title: 'Failed saving selected content',
                    method: 'error'
                },
                forbidden: {
                    content: 'You are not allowed to change the selected content in the Migration Plan.<br/>Ask an account manager for more privileges.',
                    title: 'You are not allowed to do that',
                    method: 'warning'
                },
                noaccess: {
                    content: 'You don\'t have access to this Migration Plan.',
                    title: 'You don\'t have access.',
                    method: 'error'
                },
                validaionfailder: {
                    content: 'Please Check Given Information',
                    title: 'Failed validation.',
                    method: 'error'
                }
            }
        },
        Hire: {
            save: {
                success: {
                    content: 'Selected Candidate is Hire .',
                    title: 'Candidate Hired',
                    method: 'success'
                },
                failed: {
                    content: 'Given value is Not Correct Please try again later.',
                    title: 'Failed saving selected content',
                    method: 'error'
                },
                forbidden: {
                    content: 'You are not allowed to change the selected content in the Migration Plan.<br/>Ask an account manager for more privileges.',
                    title: 'You are not allowed to do that',
                    method: 'warning'
                },
                noaccess: {
                    content: 'You don\'t have access to this Migration Plan.',
                    title: 'You don\'t have access.',
                    method: 'error'
                },
                validaionfailder: {
                    content: 'Please Check Given Information',
                    title: 'Failed validation.',
                    method: 'error'
                }
            }
        },
        OnHold: {
            save: {
                success: {
                    content: 'Selected Candidate is On-Hold .',
                    title: 'On-Hold Candidate',
                    method: 'success'
                },
                failed: {
                    content: 'Given value is Not Correct Please try again later.',
                    title: 'Failed saving selected content',
                    method: 'error'
                },
                forbidden: {
                    content: 'You are not allowed to That',
                    title: 'You are not allowed to do that',
                    method: 'warning'
                },
                noaccess: {
                    content: 'You don\'t have access to this .',
                    title: 'You don\'t have access.',
                    method: 'error'
                },
                validaionfailder: {
                    content: 'Please Check Given Information',
                    title: 'Failed validation.',
                    method: 'error'
                }
            }
        },
        connection: {
            save: {
                success: {
                    content: 'Saved was successfully saved ..',
                    title: 'Saved successfully.',
                    method: 'success'
                },
                failed: {
                    content: 'Selected content  was <strong>not</strong> saved . Please try again later.',
                    title: 'Failed saving details content',
                    method: 'error'
                },
                forbidden: {
                    content: 'You are not allowed to change the Content .',
                    title: 'You are not allowed to do that',
                    method: 'warning'
                },
                noaccess: {
                    content: 'You don\'t have access to this .',
                    title: 'You don\'t have access.',
                    method: 'error'
                }
            }
        },
        ItemTypes: {
            save: {
                success: {
                    content: 'Selected item types was successfully saved.',
                    title: 'Selected item types saved',
                    method: 'success'
                },
                failed: {
                    content: 'Selected item types was <strong>not</strong> saved . Please try again later.',
                    title: 'Failed saving selected item types',
                    method: 'error'
                },
                forbidden: {
                    content: 'You are not allowed to change the selected item types.',
                    title: 'You are not allowed to do that',
                    method: 'warning'
                },
                noaccess: {
                    content: 'You don\'t have access to this .',
                    title: 'You don\'t have access.',
                    method: 'error'
                }
            }
        },
        credentials: {
            save: {
                notModified: {
                    content: 'Credentials were not modified.',
                    title: 'Credentials were not modified',
                    method: 'warning'
                },
                success: {
                    content: 'Credentials were successfully saved.',
                    title: 'Credentials saved',
                    method: 'success'
                },
                failed: {
                    content: 'Credentials were <strong>not</strong> saved. Please try again later.',
                    title: 'Failed saving credentials',
                    method: 'error'
                },
                forbidden: {
                    content: 'You are not allowed to change This.',
                    title: 'You are not allowed to do that',
                    method: 'warning'
                },
                noaccess: {
                    content: 'You don\'t have access.',
                    title: 'You don\'t have access.',
                    method: 'error'
                }
            }
        },
        password: {
            save: {
                notModified: {
                    content: 'Failed to change passwrod, either your old password is wrong or new password is invalid .',
                    title: 'Password Change Failed',
                    method: 'error'
                },
                success: {
                    content: 'Credentials were successfully saved.',
                    title: 'Credentials saved',
                    method: 'success'
                },
                failed: {
                    content: 'Credentials were <strong>not</strong> saved. Please try again later.',
                    title: 'Failed saving credentials',
                    method: 'error'
                },
                forbidden: {
                    content: 'You are not allowed to change the credentials .<br/>Ask an account manager for more privileges.',
                    title: 'You are not allowed to do that',
                    method: 'warning'
                },
                noaccess: {
                    content: 'You don\'t have access.',
                    title: 'You don\'t have access.',
                    method: 'error'
                }
            }
        },
        migrationPlans: {
            insert: {
                success: {
                    content: 'You Invite for talent pool was  successfully send.',
                    title: 'Invite Talent Pool Send.',
                    method: 'success'
                },
                failed: {
                    content: 'You Invite for talent pool <strong>not</strong> send. Please try again later.',
                    title: 'Failed Inviting to talentpool.',
                    method: 'error'
                },
                forbidden: {
                    content: 'You are not allowed to delete.',
                    title: 'You are not allowed to do that',
                    method: 'warning'
                },
                noaccess: {
                    content: 'You don\'t have access.',
                    title: 'You don\'t have access.',
                    method: 'error'
                }
            },
            execute: {
                success: {
                    content: 'You Interview Information was  successfully send',
                    title: 'SuccessFully send',
                    method: 'success'
                },
                failed: {
                    content: 'select content was <strong>Failed</strong> . Please try again later.',
                    title: 'Failed executing Migration Plan',
                    method: 'error'
                },
                forbidden: {
                    content: 'You are not allowed.<br/>',
                    title: 'You are not allowed to do that',
                    method: 'warning'
                },
                noaccess: {
                    content: 'You don\'t have access.',
                    title: 'You don\'t have access.',
                    method: 'error'
                }
            },
            schedule: {
                success: {
                    content: 'You Reject  Information was  successfully Saved.',
                    title: 'Reject information successfully.',
                    method: 'success'
                },
                failed: {
                    content: 'Selected  was <strong>Failed</strong>. Please try again later.',
                    title: 'Failed scheduling Migration Plan',
                    method: 'error'
                },
                forbidden: {
                    content: 'You are not allowed.',
                    title: 'You are not allowed to do that',
                    method: 'warning'
                },
                noaccess: {
                    content: 'You don\'t have access.',
                    title: 'You don\'t have access.',
                    method: 'error'
                }
            },
            propertyMapping: {
                success: {
                    content: 'Property Mapping was successfully saved in the Migration Plan.',
                    title: 'Property Mapping saved',
                    method: 'success'
                },
                failed: {
                    content: 'Property Mapping was <strong>not</strong> saved in the Migration Plan. Please try again later',
                    title: 'Failed saving Property Mapping',
                    method: 'error'
                },
                forbidden: {
                    content: 'You are not allowed to change the Property Mapping in this Migration Plan.<br/>Ask an account manager for more privileges.',
                    title: 'You are not allowed to do that',
                    method: 'warning'
                },
                noaccess: {
                    content: 'You don\'t have access to this Migration Plan.',
                    title: 'You don\'t have access.',
                    method: 'error'
                }
            }
        },
        filter: {
            save: {
                notModified: {
                    content: 'Filter was not modified.',
                    title: 'Filter was not modified',
                    method: 'warning'
                },
                success: {
                    content: 'Filter was successfully saved .',
                    title: 'Filter saved',
                    method: 'success'
                },
                failed: {
                    content: 'Filter was <strong>not</strong> saved. Please try again later.',
                    title: 'Failed saving filter',
                    method: 'error'
                },
                forbidden: {
                    content: 'You are not allowed.<br/>',
                    title: 'You are not allowed to do that',
                    method: 'warning'
                },
                noaccess: {
                    content: 'You don\'t have access.',
                    title: 'You don\'t have access.',
                    method: 'error'
                }
            }
        }
    };
});
