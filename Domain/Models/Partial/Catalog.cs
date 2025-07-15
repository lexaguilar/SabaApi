using Saba.Repository;

namespace Saba.Domain.Models;

interface ICatalogo
{
}

public partial class Role : ICatalogo { }
public partial class Filial : ICatalogo { }
public partial class CatalogName : ICatalogo { }
public partial class QuestionType : ICatalogo { }
public partial class GenericCatalog : ICatalogo { }
public partial class SurveyState : ICatalogo { }
public partial class SurveyUserState : ICatalogo { }
public partial class User : ICatalogo { }
public partial class Country : ICatalogo { }

