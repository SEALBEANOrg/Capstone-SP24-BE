using Repositories.Interfaces;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class DocumentRepo : GenericRepo<Document>, IDocumentRepo
    {
        public DocumentRepo(ExagenContext context) : base(context) { }
    }
    
}
