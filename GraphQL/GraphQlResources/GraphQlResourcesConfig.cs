using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using System;

namespace GraphQlResources
{
    public static class GraphQlResourcesConfig
    {
        public static IApplicationBuilder UseGraphQlResources(this IApplicationBuilder app)
        {
            var embeddedFileProvider =
                new EmbeddedFileProvider(typeof(GraphQlResources.GraphQlResourcesConfig).Assembly, "GraphQlResources.graphql");

            var fso = new FileServerOptions
            {
                RequestPath = "/graphql",
                FileProvider = embeddedFileProvider,
                EnableDefaultFiles = true
            };

            fso.StaticFileOptions.ContentTypeProvider = new FileExtensionContentTypeProvider();

            app.UseFileServer(fso);

            return app;

        }
    }
}
