TODO: Rename this project NHibernateRepositories.  It's the Domain Port
to NHibernate-mediated SQL Server data stores, NOT EVERY DATA STORE!  For 
example, if we implemented some Repositories using MongoDB, those would
belong in a separate Project/Assembly.  There's no good reason to 
accumulate all those NuGet/assembly references in one library, and the
test code for those Repositories would likely be very different.  Instead,
we might have a folder at the Solution level named "Repositories" that
groups together all the persistence-related code.
