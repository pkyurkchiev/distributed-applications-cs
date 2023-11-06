# NuGet Package Manager Console Guide



**1. Enable migration to the project**
   - *enable-migrations -contexttypename [context.name]*

**2. Create migration**
   - *add-migration [name] -context [context.name]*

**3. Update database with applying all changes on the context**
   - *update-database -context [context.name]*

**4. Remove last migration**
   - *remove-migration -context [context.name]*
