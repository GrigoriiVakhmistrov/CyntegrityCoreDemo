using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CyntegrityDemoNetCore.ModelBinders {
    public class ObjectIdModelBinder : IModelBinder {
        public Task BindModelAsync(ModelBindingContext bindingContext) {
            var result = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);
            if (result.FirstValue != null)
                bindingContext.Result = ModelBindingResult.Success(new ObjectId(result.FirstValue));
            return Task.CompletedTask;
        }
    }

    public class ObjectIdModelBinderProvider : IModelBinderProvider {
        public IModelBinder GetBinder(ModelBinderProviderContext context) {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(ObjectId)) {
                return new BinderTypeModelBinder(typeof(ObjectIdModelBinder));
            }

            return null;
        }
    }

    public class ListObjectIdModelBinder : IModelBinder {
        public Task BindModelAsync(ModelBindingContext bindingContext) {
            var result = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);
            List<ObjectId> list = new List<ObjectId>();
            foreach (var value in result.Values) {
                list.Add(new ObjectId(value));
            }
            bindingContext.Result = ModelBindingResult.Success(list);
            return Task.CompletedTask;
        }
    }

    public class ListObjectIdModelBinderProvider : IModelBinderProvider {
        public IModelBinder GetBinder(ModelBinderProviderContext context) {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(List<ObjectId>)) {
                return new BinderTypeModelBinder(typeof(ListObjectIdModelBinder));
            }

            return null;
        }
    }
}
