# hangfire-deduplicate

Hangfire sample project to illustrate how to deduplicate jobs.
When jobs are being created, they accept a parameter which identifies an instance of the job.

A ClientFilter is responsible for checking and creating locks, based on the identification of jobs.
This makes sure only one job, based on its identification, can be created while its lock exists.

A ServerFilter is responsible for removing the lock again, when the job has run.
This makes sure jobs can be created again, based on the identification.